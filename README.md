# CourseApi — ASP.NET 8 Minimal API Backend

Бэкенд для платформы онлайн-курсов «Курсы TOP». Построен на ASP.NET 8 Minimal API + PostgreSQL (Code First через EF Core).

---

## Стек

| Слой | Технология |
|---|---|
| Фреймворк | ASP.NET 8 Minimal API |
| БД | PostgreSQL |
| ORM | Entity Framework Core 8 (Npgsql) |
| Аутентификация | JWT Bearer |
| Хэширование паролей | BCrypt.Net-Next |
| Документация | Swagger (Swashbuckle) |

---

## Структура проекта

```
CourseApi/
├── Entities/              # Сущности EF Core (по одному файлу)
│   ├── User.cs
│   ├── Course.cs
│   ├── Lesson.cs
│   ├── Test.cs
│   ├── Question.cs
│   ├── Answer.cs
│   └── UserProgress.cs
│
├── Data/
│   └── AppDbContext.cs    # DbContext + Fluent API конфигурация
│
├── Repositories/          # Слой доступа к данным
│   ├── Interfaces/
│   │   ├── IUserRepository.cs
│   │   ├── ICourseRepository.cs
│   │   ├── ILessonRepository.cs
│   │   └── IProgressRepository.cs
│   ├── UserRepository.cs
│   ├── CourseRepository.cs
│   ├── LessonRepository.cs
│   └── ProgressRepository.cs
│
├── Services/              # Бизнес-логика
│   ├── Interfaces/
│   │   ├── IAuthService.cs
│   │   ├── ICourseService.cs
│   │   ├── ILessonService.cs
│   │   └── IProgressService.cs
│   ├── AuthService.cs
│   ├── CourseService.cs
│   ├── LessonService.cs
│   └── ProgressService.cs
│
├── DTOs/                  # Data Transfer Objects
│   ├── Auth/AuthDtos.cs
│   ├── Courses/CourseDtos.cs
│   ├── Lessons/LessonDtos.cs
│   └── Progress/ProgressDtos.cs
│
├── Endpoints/             # Minimal API конечные точки
│   ├── AuthEndpoints.cs
│   ├── CourseEndpoints.cs
│   ├── LessonEndpoints.cs
│   └── ProgressEndpoints.cs
│
├── Program.cs             # Точка входа + DI + middleware
├── appsettings.json
└── CourseApi.csproj
```

---

## Быстрый старт

### 1. Требования

- [.NET 8 SDK](https://dotnet.microsoft.com/download)
- PostgreSQL 14+
- EF Core CLI: `dotnet tool install --global dotnet-ef`

### 2. Настройка базы данных

Создайте БД в PostgreSQL:

```sql
CREATE DATABASE course_db;
```

Отредактируйте `appsettings.json`:

```json
"ConnectionStrings": {
  "Default": "Host=localhost;Port=5432;Database=course_db;Username=postgres;Password=ВАШ_ПАРОЛЬ"
},
"Jwt": {
  "Key": "замените-на-строку-минимум-32-символа!!",
  "Issuer": "CourseApi",
  "Audience": "CourseFrontend"
}
```

### 3. Миграции (Code First)

```bash
# Создать первую миграцию
dotnet ef migrations add InitialCreate

# Применить миграцию к БД
dotnet ef database update
```

> `Program.cs` также вызывает `db.Database.Migrate()` при каждом запуске —
> это удобно в разработке, но для продакшена уберите этот блок.

### 4. Запуск

```bash
dotnet run
```

Приложение запустится на `http://localhost:5000`.  
Swagger UI доступен по адресу: **http://localhost:5000/swagger**

---

## API — Конечные точки

### Auth

| Метод | URL | Доступ | Описание |
|---|---|---|---|
| POST | `/api/auth/register` | Анонимный | Регистрация |
| POST | `/api/auth/login` | Анонимный | Вход, получение JWT |
| GET | `/api/auth/me` | JWT | Текущий пользователь |

**Пример: регистрация**
```json
POST /api/auth/register
{
  "name": "Иван Иванов",
  "email": "ivan@example.com",
  "password": "secret123"
}
```

**Ответ:**
```json
{
  "token": "eyJ...",
  "user": { "id": 1, "name": "Иван Иванов", "email": "ivan@example.com" }
}
```

---

### Courses

| Метод | URL | Доступ | Описание |
|---|---|---|---|
| GET | `/api/courses` | Анонимный | Список всех курсов |
| GET | `/api/courses/{id}` | Анонимный | Курс со списком уроков |

---

### Lessons

| Метод | URL | Доступ | Описание |
|---|---|---|---|
| GET | `/api/courses/{courseId}/lessons/{lessonId}` | JWT | Урок с тестом |
| POST | `/api/lessons/{lessonId}/complete` | JWT | Отметить урок пройденным |

**Пример: завершить урок**
```json
POST /api/lessons/3/complete
Authorization: Bearer <token>
{
  "score": 85.5
}
```

---

### Progress

| Метод | URL | Доступ | Описание |
|---|---|---|---|
| GET | `/api/users/me/progress` | JWT | Прогресс по всем курсам |

**Ответ:**
```json
[
  {
    "courseId": 1,
    "courseTitle": "Введение в C#",
    "completedLessons": 3,
    "totalLessons": 10
  }
]
```

---

## Схема базы данных

```
Users ──< UserProgress >── Lessons ──< Tests ──< Questions ──< Answers
                               │
                           Courses
```

- `Users` → `UserProgress`: один ко многим
- `Lessons` → `UserProgress`: один ко многим
- `Courses` → `Lessons`: один ко многим
- `Lessons` → `Tests`: один к одному
- `Tests` → `Questions`: один ко многим
- `Questions` → `Answers`: один ко многим

---

## Подключение к фронтенду

В файле `.env` React-проекта:

```env
REACT_APP_API_URL=http://localhost:5000/api
```

Фронтенд уже настроен для работы с этим URL (см. `src/services/api.js`).
