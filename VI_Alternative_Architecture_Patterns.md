# VI. Applying Alternative Architecture Patterns

## VI.1 Applying Service-Oriented Architecture (SOA)

### Problem Identification

The current layered architecture, while functional, has several limitations that prevent the system from fully meeting non-functional requirements:

**NF-05: Reusability** - The current architecture tightly couples business logic within the BLL layer, making it difficult to reuse services across different applications or platforms. Services are embedded within the same process, limiting their ability to be consumed by external systems.

**NF-06: Scalability** - The monolithic structure of the current system makes horizontal scaling challenging. All services run within the same application process, preventing independent scaling of different business functions.

**NF-07: Modularity** - The tight coupling between layers makes it difficult to modify or replace individual components without affecting the entire system.

### SOA-Based Solution

To address these limitations, we will reorganize the system components into independent, loosely-coupled services:

#### 1. Service Decomposition Strategy

**Product Management Service**
- **Responsibilities**: Product CRUD operations, inventory management, product search and filtering
- **Interfaces**: RESTful API endpoints for product operations
- **Data**: Product, ProductItem, Category entities
- **Dependencies**: Product Repository, Image Storage Service

**Order Management Service**
- **Responsibilities**: Order processing, order status management, order history
- **Interfaces**: RESTful API endpoints for order operations
- **Data**: Order, OrderItem, OrderStatus entities
- **Dependencies**: Product Management Service (for inventory checks), Payment Service

**User Management Service**
- **Responsibilities**: User authentication, authorization, profile management
- **Interfaces**: RESTful API endpoints for user operations
- **Data**: Account, AccountStatus, RoleName entities
- **Dependencies**: Authentication Service, Email Service

**Rating & Review Service**
- **Responsibilities**: Product ratings, reviews, comment management
- **Interfaces**: RESTful API endpoints for rating operations
- **Data**: Rating, Comment, CommentStatus entities
- **Dependencies**: User Management Service, Product Management Service

**Notification Service**
- **Responsibilities**: Email notifications, system alerts, user communications
- **Interfaces**: RESTful API endpoints for notification operations
- **Data**: Email templates, notification preferences
- **Dependencies**: Email Service, User Management Service

#### 2. Service Interface Design

```csharp
// Product Management Service Interface
[ApiController]
[Route("api/[controller]")]
public class ProductController : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<ProductDto>>> GetProducts([FromQuery] ProductFilterDto filter);
    
    [HttpGet("{id}")]
    public async Task<ActionResult<ProductDto>> GetProduct(int id);
    
    [HttpPost]
    public async Task<ActionResult<ProductDto>> CreateProduct([FromBody] CreateProductDto product);
    
    [HttpPut("{id}")]
    public async Task<ActionResult<ProductDto>> UpdateProduct(int id, [FromBody] UpdateProductDto product);
    
    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteProduct(int id);
}

// Order Management Service Interface
[ApiController]
[Route("api/[controller]")]
public class OrderController : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<OrderDto>>> GetOrders([FromQuery] OrderFilterDto filter);
    
    [HttpPost]
    public async Task<ActionResult<OrderDto>> CreateOrder([FromBody] CreateOrderDto order);
    
    [HttpPut("{id}/status")]
    public async Task<ActionResult<OrderDto>> UpdateOrderStatus(int id, [FromBody] UpdateOrderStatusDto status);
}
```

#### 3. Service Communication Patterns

**Synchronous Communication**: RESTful APIs for immediate responses
**Asynchronous Communication**: Message queues for event-driven operations
**Service Discovery**: Dynamic service location and load balancing

### Supporting Diagrams

#### Updated Component Diagram

```
┌─────────────────────────────────────────────────────────────────────────────┐
│                           E-commerce SOA System                            │
├─────────────────────────────────────────────────────────────────────────────┤
│                                                                             │
│  ┌─────────────────┐    ┌─────────────────┐    ┌─────────────────┐         │
│  │   MVC Client    │    │  Razor Client   │    │  Mobile Client  │         │
│  │                 │    │                 │    │                 │         │
│  ┌─────────────────┘    ┌─────────────────┘    ┌─────────────────┘         │
│                                                                             │
│  ┌─────────────────────────────────────────────────────────────────────┐   │
│  │                    API Gateway                                      │   │
│  │  ┌─────────────┐ ┌─────────────┐ ┌─────────────┐ ┌─────────────┐   │   │
│  │  │   Routing   │ │   Auth      │ │   Rate      │ │   Logging   │   │   │
│  │  │             │ │   Filter    │ │   Limiting  │ │             │   │   │
│  │  └─────────────┘ └─────────────┘ └─────────────┘ └─────────────┘   │   │
│  └─────────────────────────────────────────────────────────────────────┘   │
│                                                                             │
│  ┌─────────────────┐    ┌─────────────────┐    ┌─────────────────┐         │
│  │ Product Service │    │  Order Service  │    │  User Service   │         │
│  │                 │    │                 │    │                 │         │
│  │ • CRUD Products │    │ • Process Orders│    │ • Authentication│         │
│  │ • Inventory Mgmt│    │ • Order Status  │    │ • User Profiles │         │
│  │ • Search/Filter │    │ • Order History │    │ • Authorization │         │
│  └─────────────────┘    └─────────────────┘    └─────────────────┘         │
│                                                                             │
│  ┌─────────────────┐    ┌─────────────────┐    ┌─────────────────┐         │
│  │ Rating Service  │    │Notification Svc │    │  Payment Service│         │
│  │                 │    │                 │    │                 │         │
│  │ • Product Rating│    │ • Email Notif.  │    │ • Payment Proc. │         │
│  │ • Reviews       │    │ • System Alerts │    │ • Transaction Mgmt│        │
│  │ • Comments      │    │ • User Comm.    │    │ • Refund Handling│        │
│  └─────────────────┘    └─────────────────┘    └─────────────────┘         │
│                                                                             │
│  ┌─────────────────────────────────────────────────────────────────────┐   │
│  │                    Shared Infrastructure                            │   │
│  │  ┌─────────────┐ ┌─────────────┐ ┌─────────────┐ ┌─────────────┐   │   │
│  │  │   Database  │ │   Cache     │ │   Message   │ │   File      │   │   │
│  │  │   Layer     │ │   Service   │ │   Queue     │ │   Storage   │   │   │
│  │  └─────────────┘ └─────────────┘ └─────────────┘ └─────────────┘   │   │
│  └─────────────────────────────────────────────────────────────────────┘   │
└─────────────────────────────────────────────────────────────────────────────┘
```

#### Updated Class Diagram (Service Level)

```
┌─────────────────────────────────────────────────────────────────────────────┐
│                           Service Class Structure                          │
├─────────────────────────────────────────────────────────────────────────────┤
│                                                                             │
│  ProductService                    OrderService                            │
│  ┌─────────────────┐              ┌─────────────────┐                      │
│  │ +GetProducts()  │              │ +GetOrders()    │                      │
│  │ +GetProduct()   │              │ +CreateOrder()  │                      │
│  │ +CreateProduct()│              │ +UpdateStatus() │                      │
│  │ +UpdateProduct()│              │ +GetOrderHistory│                      │
│  │ +DeleteProduct()│              └─────────────────┘                      │
│  └─────────────────┘                        │                              │
│           │                                 │                              │
│           │                                 │                              │
│  ┌─────────────────┐              ┌─────────────────┐                      │
│  │  IProductRepo    │              │  IOrderRepo     │                      │
│  │  ┌─────────────┐ │              │  ┌─────────────┐ │                      │
│  │  │ +GetAll()   │ │              │  │ +GetAll()   │ │                      │
│  │  │ +GetById()  │ │              │  │ +GetById()  │ │                      │
│  │  │ +Add()      │ │              │  │ +Add()      │ │                      │
│  │  │ +Update()   │ │              │  │ +Update()   │ │                      │
│  │  │ +Delete()   │ │              │  │ +Delete()   │ │                      │
│  │  └─────────────┘ │              │  └─────────────┘ │                      │
│  └─────────────────┘              └─────────────────┘                      │
│                                                                             │
│  UserService                       RatingService                           │
│  ┌─────────────────┐              ┌─────────────────┐                      │
│  │ +Authenticate() │              │ +GetRating()    │                      │
│  │ +Register()     │              │ +AddRating()    │                      │
│  │ +GetProfile()   │              │ +GetComments()  │                      │
│  │ +UpdateProfile()│              │ +AddComment()   │                      │
│  │ +Authorize()    │              │ +UpdateComment()│                      │
│  └─────────────────┘              └─────────────────┘                      │
│           │                                 │                              │
│           │                                 │                              │
│  ┌─────────────────┐              ┌─────────────────┐                      │
│  │  IAccountRepo    │              │  IRatingRepo    │                      │
│  │  ┌─────────────┐ │              │  ┌─────────────┐ │                      │
│  │  │ +GetByEmail │ │              │  │ +GetRating  │ │                      │
│  │  │ +Add()      │ │              │  │ +AddRating  │ │                      │
│  │  │ +Update()   │ │              │  │ +GetComments│ │                      │
│  │  │ +Validate() │ │              │  │ +AddComment │ │                      │
│  │  └─────────────┘ │              │  └─────────────┘ │                      │
│  └─────────────────┘              └─────────────────┘                      │
└─────────────────────────────────────────────────────────────────────────────┘
```

#### Deployment Diagram

```
┌─────────────────────────────────────────────────────────────────────────────┐
│                           SOA Deployment Architecture                      │
├─────────────────────────────────────────────────────────────────────────────┤
│                                                                             │
│  Load Balancer (Nginx)                                                     │
│  ┌─────────────────────────────────────────────────────────────────────┐   │
│  │  ┌─────────────┐  ┌─────────────┐  ┌─────────────┐  ┌─────────────┐ │   │
│  │  │   Client    │  │   Client    │  │   Client    │  │   Client    │ │   │
│  │  │   Request   │  │   Request   │  │   Request   │  │   Request   │ │   │
│  │  └─────────────┘  └─────────────┘  └─────────────┘  └─────────────┘ │   │
│  └─────────────────────────────────────────────────────────────────────┘   │
│                                    │                                       │
│  API Gateway (Kong/Envoy)                                                 │
│  ┌─────────────────────────────────────────────────────────────────────┐   │
│  │  ┌─────────────┐  ┌─────────────┐  ┌─────────────┐  ┌─────────────┐ │   │
│  │  │   Auth      │  │   Rate      │  │   Routing   │  │   Logging   │ │   │
│  │  │   Service   │  │   Limiting  │  │   Service   │  │   Service   │ │   │
│  │  └─────────────┘  └─────────────┘  └─────────────┘  └─────────────┘ │   │
│  └─────────────────────────────────────────────────────────────────────┘   │
│                                    │                                       │
│  Service Discovery (Consul)                                               │
│  ┌─────────────────────────────────────────────────────────────────────┐   │
│  │  ┌─────────────┐  ┌─────────────┐  ┌─────────────┐  ┌─────────────┐ │   │
│  │  │   Service   │  │   Health    │  │   Load      │  │   Service   │ │   │
│  │  │   Registry  │  │   Checking  │  │   Balancing │  │   Catalog   │ │   │
│  │  └─────────────┘  └─────────────┘  └─────────────┘  └─────────────┘ │   │
│  └─────────────────────────────────────────────────────────────────────┘   │
│                                    │                                       │
│  Service Instances                                                         │
│  ┌─────────────────┐  ┌─────────────────┐  ┌─────────────────┐             │
│  │ Product Service │  │  Order Service  │  │  User Service   │             │
│  │  ┌─────────────┐ │  │  ┌─────────────┐ │  │  ┌─────────────┐ │             │
│  │  │ Instance 1  │ │  │  │ Instance 1  │ │  │  │ Instance 1  │ │             │
│  │  │ Port: 5001  │ │  │  │ Port: 5002  │ │  │  │ Port: 5003  │ │             │
│  │  └─────────────┘ │  │  └─────────────┘ │  │  └─────────────┘ │             │
│  │  ┌─────────────┐ │  │  ┌─────────────┐ │  │  ┌─────────────┐ │             │
│  │  │ Instance 2  │ │  │  │ Instance 2  │ │  │  │ Instance 2  │ │             │
│  │  │ Port: 5004  │ │  │  │ Port: 5005  │ │  │  │ Port: 5006  │ │             │
│  │  └─────────────┘ │  │  └─────────────┘ │  │  └─────────────┘ │             │
│  └─────────────────┘  └─────────────────┘  └─────────────────┘             │
│                                    │                                       │
│  Shared Infrastructure                                                     │
│  ┌─────────────────┐  ┌─────────────────┐  ┌─────────────────┐             │
│  │   Database      │  │   Cache         │  │   Message       │             │
│  │   (SQL Server)  │  │   (Redis)       │  │   Queue         │             │
│  │  ┌─────────────┐ │  │  ┌─────────────┐ │  │  (RabbitMQ)     │             │
│  │  │ Primary     │ │  │  │ Cache Node 1│ │  │  ┌─────────────┐ │             │
│  │  │ Instance    │ │  │  └─────────────┘ │  │  │ Queue Node 1│ │             │
│  │  └─────────────┘ │  │  ┌─────────────┐ │  │  └─────────────┘ │             │
│  │  ┌─────────────┐ │  │  │ Cache Node 2│ │  │  ┌─────────────┐ │             │
│  │  │ Secondary   │ │  │  └─────────────┘ │  │  │ Queue Node 2│ │             │
│  │  │ Instance    │ │  │  ┌─────────────┐ │  │  └─────────────┘ │             │
│  │  └─────────────┘ │  │  │ Cache Node 3│ │  │  ┌─────────────┐ │             │
│  └─────────────────┘  │  └─────────────┘ │  │  │ Queue Node 3│ │             │
│                        └─────────────────┘  │  └─────────────┘ │             │
│                                              └─────────────────┘             │
└─────────────────────────────────────────────────────────────────────────────┘
```

## VI.2 Applying Service Discovery Pattern

### Problem & Requirement

**NF-06: Scalability** - As the system grows with multiple service instances and potentially multiple branches or independent modules, the current architecture lacks the ability to dynamically discover and route requests to available service instances. This creates several challenges:

1. **Static Configuration**: Service endpoints are hardcoded, making it difficult to add or remove service instances dynamically
2. **Load Balancing**: No automatic distribution of requests across multiple service instances
3. **Health Monitoring**: No mechanism to detect and route around failed service instances
4. **Service Location**: Clients need to know the exact location of each service, creating tight coupling

**Business Requirements**:
- Support for multiple branches with independent service deployments
- Ability to scale individual services based on demand
- Automatic failover and recovery mechanisms
- Support for blue-green deployments and canary releases

### Service Discovery-Based Solution

#### 1. Service Discovery Architecture

**Service Registry Pattern**: Implement a centralized service registry that maintains a catalog of all available services and their instances.

**Client-Side Discovery**: Services register themselves with the service registry, and clients query the registry to find available service instances.

**Server-Side Discovery**: Use an API Gateway that queries the service registry to route requests to appropriate service instances.

#### 2. Technology Stack Selection

**Consul** (Primary Choice):
- **Service Registration**: Automatic service registration and deregistration
- **Health Checking**: Built-in health checking with configurable checks
- **Key-Value Store**: Distributed configuration management
- **Multi-Datacenter Support**: Native support for multiple environments
- **Web UI**: Built-in web interface for service management

**Alternative Technologies**:
- **Eureka**: Netflix's service discovery tool (Java-based)
- **Kubernetes DNS**: Native service discovery in Kubernetes environments
- **etcd**: Distributed key-value store with service discovery capabilities

#### 3. Implementation Strategy

**Service Registration**:
```csharp
// Service Registration Configuration
public class ServiceRegistrationConfig
{
    public string ServiceName { get; set; }
    public string ServiceId { get; set; }
    public string Address { get; set; }
    public int Port { get; set; }
    public string HealthCheckEndpoint { get; set; }
    public TimeSpan HealthCheckInterval { get; set; }
    public TimeSpan DeregisterAfter { get; set; }
}

// Consul Service Registration
public class ConsulServiceRegistry : IServiceRegistry
{
    private readonly IConsulClient _consulClient;
    
    public async Task RegisterServiceAsync(ServiceRegistrationConfig config)
    {
        var registration = new AgentServiceRegistration
        {
            ID = config.ServiceId,
            Name = config.ServiceName,
            Address = config.Address,
            Port = config.Port,
            Check = new AgentServiceCheck
            {
                HTTP = $"http://{config.Address}:{config.Port}{config.HealthCheckEndpoint}",
                Interval = config.HealthCheckInterval,
                DeregisterCriticalServiceAfter = config.DeregisterAfter
            }
        };
        
        await _consulClient.Agent.ServiceRegister(registration);
    }
}
```

**Service Discovery**:
```csharp
// Service Discovery Client
public class ConsulServiceDiscovery : IServiceDiscovery
{
    private readonly IConsulClient _consulClient;
    
    public async Task<List<ServiceInstance>> GetServiceInstancesAsync(string serviceName)
    {
        var queryResult = await _consulClient.Health.Service(serviceName);
        
        return queryResult.Response
            .Where(entry => entry.Checks.All(check => check.Status == HealthStatus.Passing))
            .Select(entry => new ServiceInstance
            {
                ServiceId = entry.Service.ID,
                ServiceName = entry.Service.Service,
                Address = entry.Service.Address,
                Port = entry.Service.Port
            })
            .ToList();
    }
}
```

**Load Balancing**:
```csharp
// Round-Robin Load Balancer
public class RoundRobinLoadBalancer : ILoadBalancer
{
    private readonly ConcurrentDictionary<string, int> _serviceCounters = new();
    
    public ServiceInstance SelectInstance(List<ServiceInstance> instances)
    {
        if (instances == null || !instances.Any())
            throw new InvalidOperationException("No service instances available");
            
        var serviceName = instances.First().ServiceName;
        var counter = _serviceCounters.AddOrUpdate(serviceName, 0, (key, oldValue) => oldValue + 1);
        
        return instances[counter % instances.Count];
    }
}
```

#### 4. Integration with API Gateway

**Dynamic Routing Configuration**:
```csharp
// API Gateway with Service Discovery
public class ServiceDiscoveryMiddleware
{
    private readonly RequestDelegate _next;
    private readonly IServiceDiscovery _serviceDiscovery;
    private readonly ILoadBalancer _loadBalancer;
    
    public async Task InvokeAsync(HttpContext context)
    {
        var serviceName = ExtractServiceName(context.Request.Path);
        var instances = await _serviceDiscovery.GetServiceInstancesAsync(serviceName);
        
        if (!instances.Any())
        {
            context.Response.StatusCode = 503; // Service Unavailable
            return;
        }
        
        var selectedInstance = _loadBalancer.SelectInstance(instances);
        var targetUri = new Uri($"http://{selectedInstance.Address}:{selectedInstance.Port}");
        
        // Forward request to selected service instance
        await ForwardRequestAsync(context, targetUri);
    }
}
```

#### 5. Health Checking Implementation

**Service Health Endpoints**:
```csharp
[ApiController]
[Route("health")]
public class HealthController : ControllerBase
{
    private readonly IProductService _productService;
    private readonly IDbContext _dbContext;
    
    [HttpGet]
    public async Task<IActionResult> GetHealth()
    {
        var healthChecks = new Dictionary<string, string>();
        
        // Database connectivity check
        try
        {
            await _dbContext.Database.CanConnectAsync();
            healthChecks["database"] = "healthy";
        }
        catch
        {
            healthChecks["database"] = "unhealthy";
        }
        
        // Service functionality check
        try
        {
            await _productService.GetAllProductsAsync();
            healthChecks["product_service"] = "healthy";
        }
        catch
        {
            healthChecks["product_service"] = "unhealthy";
        }
        
        var isHealthy = healthChecks.All(check => check.Value == "healthy");
        
        return isHealthy ? Ok(healthChecks) : StatusCode(503, healthChecks);
    }
}
```

#### 6. Configuration Management

**Consul Key-Value Configuration**:
```csharp
// Configuration Provider
public class ConsulConfigurationProvider : IConfigurationProvider
{
    private readonly IConsulClient _consulClient;
    
    public async Task<Dictionary<string, string>> GetConfigurationAsync(string prefix)
    {
        var queryResult = await _consulClient.KV.Get(prefix, new QueryOptions { Recurse = true });
        
        return queryResult.Response
            .ToDictionary(
                kvp => kvp.Key.Replace(prefix, ""),
                kvp => Encoding.UTF8.GetString(kvp.Value)
            );
    }
}
```

#### 7. Deployment Considerations

**Docker Compose Configuration**:
```yaml
version: '3.8'
services:
  consul:
    image: consul:latest
    ports:
      - "8500:8500"
    environment:
      - CONSUL_BIND_INTERFACE=eth0
    volumes:
      - consul-data:/consul/data
      
  product-service:
    image: ecommerce/product-service:latest
    environment:
      - CONSUL_HTTP_ADDR=consul:8500
      - SERVICE_NAME=product-service
      - SERVICE_PORT=5001
    depends_on:
      - consul
      
  order-service:
    image: ecommerce/order-service:latest
    environment:
      - CONSUL_HTTP_ADDR=consul:8500
      - SERVICE_NAME=order-service
      - SERVICE_PORT=5002
    depends_on:
      - consul

volumes:
  consul-data:
```

This service discovery implementation provides the foundation for a scalable, resilient microservices architecture that can support the growing needs of the e-commerce platform while maintaining high availability and performance. 