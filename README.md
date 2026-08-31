## ConferenceRoomBooking API
REST API для управління бронюванням та орендою конференц-залів, побудований на ASP.NET Core Web API.

## Зміст
- [Опис задачі](#запуск)
- [Технології](#технології)
- [Архітектура](#архітектура)
- [Структура проєкту](#структура-проєкту)
- [Початок роботи](#початок-роботи)
- [База даних](#база-даних)
- [API Endpoints](#api-endpoints)
- [Розрахунок вартості](#розрахунок-вартості)
- [Тестування](#тестування)
- [Відомі обмеження](#відомі-обмеження)

## Опис задачі
Компанія надає в оренду конференц-зали для бізнесу. API дозволяє клієнтам шукати доступні зали, бронювати їх, а також розраховувати вартість оренди залежно від часу та обраних послуг.
## Бізнес-можливості:
- управління каталогом залів (створення, редагування, деактивація)
- пошук доступних залів за датою/часом і місткістю
- бронювання залу з обраними додатковими послугами
- автоматичний розрахунок вартості оренди залежно від тарифного поясу
- аналітичні звіти для бізнесу (завантаженість, дохід, популярність послуг)
## Технології
| Категорія | Технологія |
| Платформа | .NET 10 / C# |
| Веб-фреймворк | ASP.NET Core Web API |
| ORM | Entity Framework Core 10 (SQL Server) |
| Медіатор | Mediator (source-generated, без reflection) |
| Валідація | FluentValidation |
| Документація API | Swagger / OpenAPI |
| Тести | xUnit + FluentAssertions |

## Архітектура
ConferenceBooking.Domain             — ядро, без залежностей від інших шарів
    ↑
ConferenceBooking.Application        — use cases (CQRS через Mediator), валідація
    ↑
ConferenceBooking.Infrastructure     — EF Core, репозиторії, міграції
    ↑
ConferenceRoomBooking                — ASP.NET Core Web API (Presentation)

## Патерни
- **CQRS** — розділення команд і запитів через Mediator, кожен use case в окремому файлі (Command/Query + Validator + Handler)
- **Repository** — абстракція доступу до даних (IRoomRepository, IBookingRepository, IServiceRepository)
- **Pipeline Behaviors** — наскрізна логіка (валідація, логування) через IPipelineBehavior
- **Value Objects** — Money, TimeRange інкапсулюють власні інваріанти
- **Aggregate Root** — окремі агрегати, щоб перевірка перетинів бронювань і конкурентного доступу не блокувала весь зал
- **Soft delete** — зали та послуги не видаляються фізично, щоб не зламати історію вже здійснених бронювань

## Доменна модель
Room (Зал)
  ├── RoomService (зв'язок з послугами)
  └── Booking (бронювання, окремий агрегат)
Service (послуга)

## Структура проєкту
ConferenceRoomBooking.sln
├── ConferenceBooking.Domain/          # Доменний шар
│   ├── Common/                        # AggregateRoot, DomainErrorMessages
│   ├── Entities/                      # Room, Service, Booking, RoomService
│   ├── Enums/                         # BookingStatus, RateBand
│   ├── Exceptions/                    # DomainException
│   ├── Interfaces/                    # IRoomRepository, IBookingRepository, IServiceRepository
│   ├── Services/                      # PricingService  
│   └── ValueObjects/                  # Money, TimeRange
│
├── ConferenceBooking.Application/     # Application шар (CQRS, валідація)
│   ├── Rooms/                         # Commands: Create/Update/Delete/AddService, Queries: Search/GetById
│   ├── Bookings/                      # Commands: Create
│   ├── Reports/                       # Queries: Occupancy/Revenue/PopularServices
│   └── Common/                        # Behaviors, ErrorMessages, Exceptions
│
├── ConferenceBooking.Infrastructure/  # EF Core, репозиторії, міграції
│   └── Persistence/                   # AppDbContext, Configurations, Repositories, Seed
│
├── ConferenceRoomBooking/             # ASP.NET Core Web API
│   ├── Controllers/                   # RoomsController, BookingsController, ReportsController
│   ├── Middleware/                    # ExceptionHandlingMiddleware
│   └── Program.cs
│
└── tests/
    ├── ConferenceBooking.Domain.Tests/        # Юніт-тести доменної логіки
    └── ConferenceBooking.Application.Tests/   # Юніт-тести хендлерів (in-memory EF Core)
