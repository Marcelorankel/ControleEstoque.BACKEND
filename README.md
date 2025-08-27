Projeto está em .net core 9.0 e usa mySQL

O script das tabelas está na raiz do projeto do git nome : ControleEstoqueTabelas.sql

Possui Tracing, APM.

Possui microservice RABBITMQ na requisição de novo PEDIDO, com tratamento de rollback.

Projeto de UNIT teste unit real, com teste de service de cadastro de novo usuario.

Instalar DOCKER windows

Executar comando CMD : docker run -d --name rabbitmq -p 5672:5672 -p 15672:15672 rabbitmq:management

LOCALHOST RabbitMQ : http://localhost:15672/
