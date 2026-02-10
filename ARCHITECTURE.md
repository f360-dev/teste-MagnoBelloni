## Architecture

O projeto est� organizado da seguinte forma:

- **F360.Domain**: Entidades, enums e exceptions
- **F360.Application**: Use cases, DTOs, interfaces, e validators
- **F360.Infrastructure**: Database, messaging, e HTTP clients
- **F360.Api**: API e middlewares
- **F360.Workers.OutboxWorker**: Background service para o outbox pattern
- **F360.Workers.JobConsumer**: RabbitMQ consumer (Filas de prioridade high e low)

A Api � a entrada para o restante dos componentes,onde � realizado o registro de jobs e salvo na tabela de outbox messages.
O OutboxWorker consulta as outbox messages, registrar lock para impedir que outros pods conflitem e por fim dispara para o RabbitMQ.
O JobConsumer � respons�vel por consumir as mensagens do RabbitMQ e process�-las de acordo com a prioridade definida.

![Diagrama](https://github.com/MagnoBelloni/F360/blob/main/doc/diagram.png)