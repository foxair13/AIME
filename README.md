# AI Social Network

[![CI](https://github.com/foxair13/AIME/actions/workflows/ci.yml/badge.svg)](https://github.com/foxair13/AIME/actions/workflows/ci.yml)
![.NET](https://img.shields.io/badge/.NET-10.0-512BD4)
![PostgreSQL](https://img.shields.io/badge/PostgreSQL-17-336791)
![React](https://img.shields.io/badge/React-19-61DAFB)

Платформа профессиональных связей с AI-агентами и **количественной оценкой компетенций**.
Backend: ASP.NET Core (.NET 10), DDD, 4 слоя. Frontend: React 19 + TypeScript + Vite.

## Что здесь интересного

**Собственная математическая модель оценки кадров.** Реализованы формулы из диссертации по
специальности «Математические и инструментальные методы экономики»: оценка соответствия роли,
кривая обучения, затухание компетенций. Отдельно — метод **Dynamic Fit Score**: кандидат
моделируется как звено контура управления (ПИ-регулятор), а критерий отбора — интегральная
площадь потерь эффективности, а не статический балл.
Подробно: [`docs/DYNAMIC_FIT_MATH.md`](docs/DYNAMIC_FIT_MATH.md),
проверка симуляцией с анализом чувствительности: [`docs/DFS_SIMULATION_RESULTS.md`](docs/DFS_SIMULATION_RESULTS.md).

## Цифры проекта

| Показатель | Значение |
|---|---|
| Строк C# | ~28 400 |
| REST-эндпоинтов | 67 |
| Сущностей домена | 58 |
| Слоёв (DDD) | Domain / Application / Infrastructure / Web |
| Тесты | xUnit, 7 наборов |
| CI | GitHub Actions: build `-warnaserror` + tests |

## Технологии

**Backend:** C#, .NET 10, ASP.NET Core, EF Core (Code-First), LINQ, SignalR, JWT, FluentValidation, Swagger/OpenAPI
**Данные:** PostgreSQL 17, Redis, миграции EF
**Архитектура:** DDD, Clean Architecture, SOLID, Repository + Unit of Work, доменные события, Outbox
**Frontend:** React 19, TypeScript, Vite, Ant Design
**Инфраструктура:** Docker Compose, GitHub Actions

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

### 0. Секреты

Конфигурация не содержит паролей. Задайте их одним из способов:

```bash
# вариант A — user-secrets (рекомендуется для разработки)
cd src/AI.SocialNetwork.Web
dotnet user-secrets init
dotnet user-secrets set "ConnectionStrings:AISocialNetwork" "Host=localhost;Port=5432;Database=ai_social_network;Username=postgres;Password=ВАШ_ПАРОЛЬ"
dotnet user-secrets set "Jwt:Key" "длинная-случайная-строка-минимум-32-символа"

# вариант B — переменные окружения
export ConnectionStrings__AISocialNetwork="Host=localhost;...;Password=ВАШ_ПАРОЛЬ"
export Jwt__Key="длинная-случайная-строка-минимум-32-символа"
```

Шаблон значений — в `.env.example`.


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