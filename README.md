Projeto está em .net core 9.0 e usa mySQL

O script das tabelas está na raiz do projeto do git nome : ControleEstoqueTabelas.sql

Possui SWAGGER

Possui Tracing, APM.

Possui regras Authorization Administrador e Vendedor
Todos end points necessitam de usuario logado com token valido, com excessao do Cadastro de novo usuario e auth.

O Endpoint AtualizarProdutoAdminEstoque necessita de LOGIN Administrador para ter acesso a atualização de produto e estoque.

Possui microservice RABBITMQ na requisição de novo PEDIDO, com tratamento de rollback.

Projeto de UNIT teste unit real, com teste de service de cadastro de novo usuario persistindo na base.

Instalar DOCKER windows
Iniciar docker e verificar se RabbitMQ está rodando, caso n esteja executar.

Executar comando CMD : docker run -d --name rabbitmq -p 5672:5672 -p 15672:15672 rabbitmq:management

LOCALHOST RabbitMQ : http://localhost:15672/

PASSO A PASSO DA API

//USUARIO
1 – Cadastro usuario                    /api/Usuario/Cadastrar
Informar:
Nome
Email
Senha (Maior ou igual a 6 caracteres)
Selecionar tipo Usuario “Admin” ou Vendedor
2- GetAll                                                /api/Usuario/Cadastrar
Retorna todos os usuarios cadastrados no sistema
3- GetByID        /api/Usuario/GetByID
Retorna o usuario de acordo com o ID do usuario buscado 

//AUTHENTICACAO
1– Authenticar Usuario              /api/Auth/Login
Informar:
Email
Senha
Retorno : Sistema retornara TOKEN com role acesso de acordo com o tipo de usuario logado (admin ou vendedor)

//PRODUTO
1- Cadastrar Produto                 /api/Produto/Cadastrar
Informar:
Nome
Descricao
Preço

2- AtualizarProdutoEstoque      /api/Produto/AtualizarProdutoEstoque
Permissão geral , qualquer usuario pode alterar o produto, menos a quantidade no estoque.
Informar
Id do produto
Nome
Descricao
Preço

3- AtualizarProdutoEstoque      /api/Produto/AtualizarProdutoAdminEstoque
Permissão somente com usuario ADMINISTRADOR , pode alterar o produto e também a quantidade do produto no estoque.

4- GetAll(Listar todos os produtos)   /api/Produto/GetAll
Retorna todos os produtos cadastrados no estoque.

5- GetByID                   /api/Produto/GetByID
Retorna o produto de acordo com o ID do produto buscado 
