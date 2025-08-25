# Микросервис "Банковские счета"

## UseCase

- менеджер банка Анна, открыла клиенту Ивану бесплатный текущий счёт, чтобы он мог хранить средства.

## 🔑 Основные элементы

1. **Доменная модель (`Account`, `Transaction`)**

    * `Account` отражает сущность счёта с балансом, валютой, ставкой, владельцем и транзакциями.
    * Поддерживаются типы счётов (`Deposit`, `Checking`, `Credit`).
    * Есть признак блокировки `IsFrozen` и soft-delete через `ClosedAt`.
    * `Transaction` хранит движения по счёту, включая `CounterPartyAccountId` для внутренних переводов.

2. **DTO + Валидация (`CreateAccountDto`, `CreateAccountValidator`)**

    * Разделение DTO и сущностей — 👍.
    * FluentValidation грамотно проверяет бизнес-правила:

        * депозит/расчётный: баланс ≥ 0
        * кредитный: баланс ≤ 0
        * процентная ставка обязательна для депозита/кредита
        * код валюты ISO 4217.

3. **CQRS + Mediator**

    * `CreateAccountRequest` → `CreateAccountHandler`.
    * Handler создаёт аккаунт, пишет событие `AccountOpened` в outbox, фиксирует транзакцию.

4. **Событийная модель**

    * `AccountOpened` публикуется через Outbox.
    * `CreateAccountProducer` вытаскивает события из базы и шлёт в RabbitMQ (с retry и jitter).
    * `CreateAccountConsumer` слушает события и пишет в Inbox для идемпотентности.
    * Используется **Outbox / Inbox паттерн + EventEnvelope (correlation, causation, source)** — это уже уровень серьёзной **event-driven архитектуры** 👍.

5. **Инфраструктура**

    * EF Core (`ModuleBankAppContext`) с конфигурациями, индексами (HASH по `OwnerId`), row versioning через `xmin`.
    * `CorrelationIdMiddleware` для сквозной трассировки.
    * EventBus с RabbitMQ.

6. **API Endpoint**

    * `POST /account` создаёт счёт.
    * Возвращает `201 Created` + DTO.
    * Требует авторизации и привязывает `OwnerId` к Claims.

    
🚀 Что получилось

- менеджер банка Анна, открыла клиенту Ивану срочный вклад «Надёжный‑6» под 3% годовых, чтобы он смог накопить средства.
- кассир банка Алексей, пополнил текущий счёт клиента Ивана на 1000 рублей наличными.
- клиент банка Иван, перевёл 200 рублей со своего текущего счёта на вклад «Надёжный‑6», чтобы пополнить вклад.

![usecase.drawio.png](images/usecase.drawio.png)

# Запуск в docker

```
docker-compose up --build
```
**Замечание**: загрузка сервиса аутентификации `Keycloak` может занять некоторое время,
т.к. будет происходить конфигурация пользователя

# Swagger

```
http://localhost/swagger
```

# Тестовые данные

POST /auth/login - Аутентификация

```json
{
  "username": "user1",
  "password": "123"
}
```
