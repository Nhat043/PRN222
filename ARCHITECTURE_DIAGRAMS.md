# PRN222 Architecture Visualization

## System Architecture Overview

```
┌─────────────────────────────────────────────────────────────┐
│                    PRESENTATION LAYER                       │
├─────────────────────────────┬───────────────────────────────┤
│         MVC Project         │       Razor Project           │
│    (Customer Interface)     │     (Admin Interface)         │
│                             │                               │
│  • Product Catalog          │  • Product Management         │
│  • Shopping Cart            │  • Order Management           │
│  • User Registration        │  • User Administration        │
│  • Order Placement          │  • Statistics & Reports       │
│  • Payment Processing       │  • System Configuration       │
│                             │                               │
│  Routes: /mvc/*             │  Routes: /razor/*             │
└─────────────────────────────┴───────────────────────────────┘
                              │
                    ┌─────────▼─────────┐
                    │   SignalR Hub     │
                    │ (Real-time Comm)  │
                    └─────────┬─────────┘
                              │
┌─────────────────────────────▼─────────────────────────────────┐
│                  BUSINESS LOGIC LAYER (BLL)                  │
├───────────────────────────────────────────────────────────────┤
│                        Services                              │
│  ┌─────────────┐ ┌──────────────┐ ┌─────────────────────┐   │
│  │   Account   │ │   Product    │ │       Order         │   │
│  │   Service   │ │   Service    │ │      Service        │   │
│  └─────────────┘ └──────────────┘ └─────────────────────┘   │
│  ┌─────────────┐ ┌──────────────┐ ┌─────────────────────┐   │
│  │   Rating    │ │  Category    │ │    Statistics       │   │
│  │   Service   │ │   Service    │ │      Service        │   │
│  └─────────────┘ └──────────────┘ └─────────────────────┘   │
│                                                               │
│                    Design Patterns                           │
│  ┌─────────────────────────────────────────────────────────┐ │
│  │              Price Calculator                           │ │
│  │  ┌─────────────────┐    ┌─────────────────┐            │ │
│  │  │ BasePriceCalc   │◄───┤ DiscountDecorator│            │ │
│  │  │                 │    │                 │            │ │
│  │  └─────────────────┘    └─────────────────┘            │ │
│  └─────────────────────────────────────────────────────────┘ │
│                                                               │
│                        Utilities                             │
│  ┌─────────────┐ ┌──────────────┐ ┌─────────────────────┐   │
│  │   Email     │ │    Image     │ │     Password        │   │
│  │   Helper    │ │   Helper     │ │   Hashing Helper    │   │
│  └─────────────┘ └──────────────┘ └─────────────────────┘   │
└───────────────────────────────────────────────────────────────┘
                              │
┌─────────────────────────────▼─────────────────────────────────┐
│                  DATA ACCESS LAYER (DAL)                     │
├───────────────────────────────────────────────────────────────┤
│                      Repository Pattern                      │
│  ┌─────────────┐ ┌──────────────┐ ┌─────────────────────┐   │
│  │  IAccountRepo│ │ IProductRepo │ │    IOrderRepo       │   │
│  │      ▲      │ │      ▲       │ │         ▲           │   │
│  │ AccountRepo │ │ ProductRepo  │ │    OrderRepo        │   │
│  └─────────────┘ └──────────────┘ └─────────────────────┘   │
│                                                               │
│                      Entity Models                           │
│  ┌─────────────┐ ┌──────────────┐ ┌─────────────────────┐   │
│  │   Account   │ │   Product    │ │       Order         │   │
│  │             │ │              │ │                     │   │
│  │ • Email     │ │ • Name       │ │ • OrderDate         │   │
│  │ • Password  │ │ • Picture    │ │ • TotalAmount       │   │
│  │ • Role      │ │ • Category   │ │ • Status            │   │
│  └─────────────┘ └──────────────┘ └─────────────────────┘   │
│                                                               │
│                     Entity Framework                         │
│  ┌─────────────────────────────────────────────────────────┐ │
│  │                  DemoContext                            │ │
│  │                                                         │ │
│  │  • DbSet<Account> Accounts                              │ │
│  │  • DbSet<Product> Products                              │ │
│  │  • DbSet<Order> Orders                                  │ │
│  │  • Entity Relationships & Configurations               │ │
│  └─────────────────────────────────────────────────────────┘ │
└───────────────────────────────────────────────────────────────┘
                              │
┌─────────────────────────────▼─────────────────────────────────┐
│                      SQL SERVER DATABASE                     │
├───────────────────────────────────────────────────────────────┤
│                        Core Tables                           │
│  ┌─────────────┐ ┌──────────────┐ ┌─────────────────────┐   │
│  │  Accounts   │ │   Products   │ │      Orders         │   │
│  │             │ │              │ │                     │   │
│  │ Categories  │ │ ProductItems │ │    OrderItems       │   │
│  │             │ │              │ │                     │   │
│  │  Ratings    │ │  Comments    │ │   OrderStatus       │   │
│  └─────────────┘ └──────────────┘ └─────────────────────┘   │
│                                                               │
│                      Supporting Tables                       │
│  ┌─────────────┐ ┌──────────────┐ ┌─────────────────────┐   │
│  │ RoleNames   │ │ Variations   │ │   RevenueLog        │   │
│  │AccountStatus│ │VariationOpts │ │   CommentStatus     │   │
│  │ProductStatus│ │ProductItemSts│ │                     │   │
│  └─────────────┘ └──────────────┘ └─────────────────────┘   │
└───────────────────────────────────────────────────────────────┘
```

## Communication Flow

```
Customer Journey:
┌──────────┐    ┌──────────┐    ┌──────────┐    ┌──────────┐
│   MVC    │───▶│   BLL    │───▶│   DAL    │───▶│ Database │
│Interface │    │ Services │    │Repository│    │SQL Server│
└──────────┘    └──────────┘    └──────────┘    └──────────┘
     │               │
     ▼               ▼
┌──────────┐    ┌──────────┐
│ SignalR  │───▶│  Razor   │
│   Hub    │    │  Admin   │
└──────────┘    └──────────┘

Real-time Notification Flow:
Customer Order → MVC Controller → OrderService → SignalR Hub → Admin Interface
```

## Design Pattern Implementation

```
Decorator Pattern (Price Calculation):
┌─────────────────┐    ┌─────────────────┐    ┌─────────────────┐
│ IPriceCalculator│◄───┤BasePriceCalcul │◄───┤DiscountDecorator│
│   Interface     │    │     ator        │    │                 │
└─────────────────┘    └─────────────────┘    └─────────────────┘
                                                      ▲
                                                      │
                                               ┌─────────────────┐
                                               │  Future: Tax    │
                                               │  Decorator      │
                                               └─────────────────┘

Repository Pattern (Data Access):
┌─────────────────┐    ┌─────────────────┐
│   IRepository   │◄───┤ ConcreteRepo    │
│   Interface     │    │ Implementation  │
└─────────────────┘    └─────────────────┘
                              │
                              ▼
                       ┌─────────────────┐
                       │  Entity         │
                       │  Framework      │
                       └─────────────────┘
```

## Technology Integration

```
External Services:
┌──────────┐    ┌──────────┐    ┌──────────┐
│  PayPal  │───▶│   MVC    │◄───│ MailKit  │
│   SDK    │    │  App     │    │  Email   │
└──────────┘    └──────────┘    └──────────┘
                      │
                      ▼
                ┌──────────┐
                │ Shared   │
                │ Images   │
                └──────────┘

Development Stack:
┌──────────┐    ┌──────────┐    ┌──────────┐
│ .NET 8.0 │───▶│ASP.NET   │───▶│Entity    │
│   Core   │    │   Core   │    │Framework │
└──────────┘    └──────────┘    └──────────┘
                      │              │
                      ▼              ▼
                ┌──────────┐    ┌──────────┐
                │ SignalR  │    │SQL Server│
                │Real-time │    │Database  │
                └──────────┘    └──────────┘
```