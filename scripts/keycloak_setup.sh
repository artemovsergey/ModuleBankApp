#!/bin/bash

# 1. Запускаем Keycloak в фоновом режиме
echo "Starting Keycloak in background..."
/opt/keycloak/bin/kc.sh start-dev --http-enabled=true --hostname-strict=false &

# 2. Функция проверки доступности
wait_for_keycloak() {
    local attempts=0
    local max_attempts=20
    
    until [ $attempts -ge $max_attempts ] || \
          /opt/keycloak/bin/kcadm.sh config credentials \
            --server http://localhost:8080 \
            --realm master \
            --user admin \
            --password admin \
            --client admin-cli 2>/dev/null
    do
        attempts=$((attempts + 1))
        echo "Loading Keycloak... (attempt $attempts/$max_attempts)"
        sleep 5
    done
    
    [ $attempts -lt $max_attempts ]
}

# 3. Ожидаем готовности
if ! wait_for_keycloak; then
    echo "Failed to start Keycloak within $max_attempts attempts"
    exit 1
fi

# 4. Основные настройки
echo "Configuring Keycloak..."

# Выполняем настройку master realm (disable sslRequired)
echo "Disabling HTTPS requirement..."
/opt/keycloak/bin/kcadm.sh update realms/master -s sslRequired=NONE

echo "Создаем Realm"
/opt/keycloak/bin/kcadm.sh create realms -s realm=ModulBankApp -s enabled=true -s sslRequired=NONE

echo "Создаем Client Scope (если его нет)"
/opt/keycloak/bin/kcadm.sh create client-scopes \
  -r ModulBankApp \
  -s name=audience-scope \
  -s protocol=openid-connect

echo "Создаем клиента с Direct Access Grants"
/opt/keycloak/bin/kcadm.sh create clients -r ModulBankApp -s clientId=backend-api -s enabled=true \
  -s directAccessGrantsEnabled=true -s publicClient=true -s 'defaultClientScopes=["audience-scope"]'

echo "Создаем пользователя"
/opt/keycloak/bin/kcadm.sh create users -r ModulBankApp -s username=user1 -s email=user1@test.com -s enabled=true -s firstName="firstname" -s lastName="lastname"
/opt/keycloak/bin/kcadm.sh set-password -r ModulBankApp --username user1 --new-password 123

echo "Добавляем Audience Mapper"
# Используем временный файл для получения ID scope
/opt/keycloak/bin/kcadm.sh get client-scopes -r ModulBankApp > /tmp/scopes.json
SCOPE_ID=$(grep -B 2 "audience-scope" /tmp/scopes.json | grep id | cut -d'"' -f4)

/opt/keycloak/bin/kcadm.sh create "client-scopes/$SCOPE_ID/protocol-mappers/models" \
  -r ModulBankApp \
  -s name=audience-mapper \
  -s protocol=openid-connect \
  -s protocolMapper=oidc-audience-mapper \
  -s config="{\"included.client.audience\": \"backend-api\", \"id.token.claim\": \"true\", \"access.token.claim\": \"true\"}"

echo "Добавляем User ID Mapper для включения sub в токен"
# Получаем ID клиента backend-api
CLIENT_ID=$(/opt/keycloak/bin/kcadm.sh get clients -r ModulBankApp --query clientId=backend-api --fields id --format 'csv' --noquotes)

# Добавляем маппер для ID пользователя
/opt/keycloak/bin/kcadm.sh create clients/$CLIENT_ID/protocol-mappers/models \
  -r ModulBankApp \
  -s name=user-id-mapper \
  -s protocol=openid-connect \
  -s protocolMapper=oidc-usermodel-property-mapper \
  -s config="{\"user.attribute\":\"id\",\"claim.name\":\"sub\",\"jsonType.label\":\"string\",\"id.token.claim\":true,\"access.token.claim\":true}"

# Переводим процесс в foreground
echo "Keycloak is fully configured and ready"
wait $KC_PID