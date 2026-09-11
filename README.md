# AI Social Network

Цифровые двойники + AI-агенты, репутация через сделки (escrow), дерево компетенций MVCDIS.
Backend: ASP.NET Core Web API (net10.0) + PostgreSQL + EF Core Code-First. Frontend: React + TypeScript + Vite + Ant Design.

## Состав решения

| Проект | Назначение |
|---|---|
| `src/AI.SocialNetwork.Domain` | Сущности, ValueObjects, события (DDD) |
| `src/AI.SocialNetwork.Application` | Интерфейсы и сервисы поверх IUnitOfWork (11 сервисов, включая MathService/CompetenceService) |
| `src/AI.SocialNetwork.Infrastructure` | DbContext, EF-конфигурации, GenericRepository, UnitOfWork |
| `src/AI.SocialNetwork.Web` | Web API: контроллеры, JWT, FluentValidation, Swagger |
| `src/AI.SocialNetwork.Tests` | xUnit-тесты (MathService, DealService) |
| `client/` | React 18 + TS + Vite + Ant Design (вход/регистрация/пользователи/профиль) |

## Быстрый старт

### 1. База данных

Вариант A — Docker (рекомендуется):

```bash
docker compose up -d
```

Вариант B — локальный PostgreSQL 17 (служба `postgresql-X64-17`), параметры в `appsettings.json`.

Создание таблиц из модели (Code-First):

```bash
dotnet tool install --global dotnet-ef
cd src/AI.SocialNetwork.Web
dotnet ef database update
```

### 2. Backend (порт 5215)

```bash
cd src/AI.SocialNetwork.Web
dotnet run
```

Swagger: http://localhost:5215/swagger/index.html

### 3. Frontend (порт 5173, прокси на :5215)

```bash
cd client
npm install
npm run dev
```

Открыть http://localhost:5173

### 4. Тесты

```bash
dotnet test
```

## Штрихи архитектуры

- Репозитории НЕ вызывают SaveChanges: commit выполняют сервисы через `IUnitOfWork.CommitAsync()`.
- СУБД генерируется из модели: единый DbContext → `dotnet-ef migrations add` → `database update`.
- 55 таблиц (пользователи, навыки/closure, сделки/escrow/milestones, чаты, уведомления, KYC, монитизация, webhooks, права агентов…).
- JWT-аутентификация (BCrypt-хэши паролей), роли `user` / `moderator` / `admin`.
- Ошибки по RFC 7807 (ProblemDetails), валидация FluentValidation.
- Формулы MVCDIS: RoleFit (3.1), Normir (3.2/3.3), кривая обучения/забывания (3.27–3.30), Гаверсин.

## Структура

```
AI.SocialNetwork.slnx
├─ src/
│  ├─ AI.SocialNetwork.Domain/        # сущности
│  ├─ AI.SocialNetwork.Application/   # сервисы + контракты
│  ├─ AI.SocialNetwork.Infrastructure/ # EF + репозитории + UoW
│  ├─ AI.SocialNetwork.Web/           # API + DI + миграции
│  └─ AI.SocialNetwork.Tests/         # xUnit
├─ client/                            # React
└─ docker-compose.yml                 # postgres/redis/ollama
```