- отредактировать swagger хорошо
- сделать версионирование api
- настроить кеширование через `redis`
- применить `ratelimiting` and `throttling`
- health endpoint
- loadbalancer
- monitoring
- date type in format


# EF
- применить в контексте `OnConfiguring`, разгрузив `Program.cs`
- можно не использовать с навигационным свойством и id, оставить только навигационное свойство, но применять метод `Attach`
- метод `Find` не делает доп. запросы, при отслеживании сущности
- работа с транзакциями

