CREATE TABLE `tbusuario` (
  `id` char(36) NOT NULL,
  `nome` varchar(100) NOT NULL,
  `email` varchar(50) NOT NULL,
  `senha` varchar(20) NOT NULL,
  `tipoUsuario` int NOT NULL DEFAULT '0',
  `createdAt` datetime DEFAULT NULL,
  `updatedAt` datetime DEFAULT NULL,
  `deletedAt` datetime DEFAULT NULL,
  PRIMARY KEY (`id`)
);

CREATE TABLE `tbproduto` (
  `id` char(36) NOT NULL,
  `nome` varchar(50) NOT NULL,
  `descricao` varchar(100) NOT NULL,
  `preco` decimal(10,2) DEFAULT NULL,
  `quantidade` int DEFAULT NULL,
  `createdAt` datetime DEFAULT NULL,
  `updatedAt` datetime DEFAULT NULL,
  `deletedAt` datetime DEFAULT NULL,
  PRIMARY KEY (`id`)
);

CREATE TABLE `tbpedido` (
  `id` char(36) NOT NULL,
  `idUsuario` char(36) NOT NULL,
  `documentoCliente` varchar(15) DEFAULT NULL,
  `dataPedido` datetime NOT NULL,
  `valorTotal` decimal(10,2) NOT NULL,
  `createdAt` datetime DEFAULT NULL,
  `updatedAt` datetime DEFAULT NULL,
  `deletedAt` datetime DEFAULT NULL,
  PRIMARY KEY (`id`),
  KEY `FK_tbPedido_Usuario` (`idUsuario`),
  CONSTRAINT `FK_tbPedido_Usuario` FOREIGN KEY (`idUsuario`) REFERENCES `tbusuario` (`id`)
);

CREATE TABLE `tbprodutopedido` (
  `id` char(36) NOT NULL,
  `idPedido` char(36) NOT NULL,
  `idProduto` char(36) NOT NULL,
  `createdAt` datetime DEFAULT NULL,
  `updatedAt` datetime DEFAULT NULL,
  `deletedAt` datetime DEFAULT NULL,
  PRIMARY KEY (`id`),
  KEY `FK_tbProdutoPedido_Pedido` (`idPedido`),
  KEY `FK_tbProdutoPedido_Produto` (`idProduto`),
  CONSTRAINT `FK_tbProdutoPedido_Pedido` FOREIGN KEY (`idPedido`) REFERENCES `tbpedido` (`id`),
  CONSTRAINT `FK_tbProdutoPedido_Produto` FOREIGN KEY (`idProduto`) REFERENCES `tbproduto` (`id`)
);