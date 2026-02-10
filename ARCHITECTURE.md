## Architecture

O projeto está organizado da seguinte forma:

- **F360.Domain**: Entidades, enums e exceptions
- **F360.Application**: Use cases, DTOs, interfaces, e validators
- **F360.Infrastructure**: Database, messaging, e HTTP clients
- **F360.Api**: API e middlewares
- **F360.Workers.OutboxWorker**: Background service para o outbox pattern
- **F360.Workers.JobConsumer**: RabbitMQ consumer (Filas de prioridade high e low)

A Api é a entrada para o restante dos componentes,onde é realizado o registro de jobs e salvo na tabela de outbox messages.
O OutboxWorker consulta as outbox messages, registrar lock para impedir que outros pods conflitem e por fim dispara para o RabbitMQ.
O JobConsumer é responsável por consumir as mensagens do RabbitMQ e processá-las de acordo com a prioridade definida.

[Diagrama](https://github.com/MagnoBelloni/MagnoBelloni/blob/main/docs/diagram.png)