# XmlProcessingSystem

Система для обработки XML-файлов, публикации сообщений в RabbitMQ и сохранения состояния модулей в SQLite.

---

## Установка и запуск

```bash
cd XmlProcessingSystem
docker compose up --build
```

> Сервисы запустятся после запуска rabbitmq (healthcheck на уровне docker compose)

---

## Конфигурация

### .env

Все параметры подключения находятся в `.env`. Пример — в `.env.template`.

```env
RABBITMQ_USER=guest
RABBITMQ_PASSWORD=guest
RABBITMQ_PORT=5672
RABBITMQ_MANAGEMENT_PORT=15672
RABBITMQ_QUEUE=xmlqueue

DB_CONNECTION=Data Source=data.db
```

> Убедитесь, что `.env` скопирован из `.env.template` и заполнен корректно.

---

### FileParserService

Конфигурация мониторинга файлов — в `XmlProcessingSystem/FileParserService/appsettings.json`.

```json
{
  "Monitor": {
    "WatchDirectory": "/home/data",
    "WatchDelayMiliseconds": 1000
  }
}
```

> Путь /home/data смонтирован с папкой XmlProcessingSystem/data в докере

---

## База данных

SQLite-база автоматически создаётся и инициализируется при запуске `DataProcessorService`, если её нет.

- Таблица: `Modules`
- Поля: `ModuleCategoryID`, `ModuleState`

---

## RabbitMQ

При запуске `RabbitMQMessageBus`:

- Подключение к очереди `xmlqueue`
- Очередь создаётся, если не существует
- Публикация и подписка на сообщения в UTF-8

---

## Структура проекта

```
XmlProcessingSystem/
├── FileParserService/          # Мониторинг и парсинг XML
├── DataProcessorService/       # Обработка и сохранение данных
├── XmlProcessingSystem.Shared/ # Общие модели и интерфейсы
├── data                        # Папка по умолчанию с xml данными
├── docker-compose.yml
├── .env.template
```

---

## Проверка доступности

- SQLite: проверяется при старте `ModuleRepository`
- RabbitMQ: проверяется через `QueueDeclarePassive`
