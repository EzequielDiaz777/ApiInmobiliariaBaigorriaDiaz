-- phpMyAdmin SQL Dump
-- version 5.1.0
-- https://www.phpmyadmin.net/
--
-- Servidor: 127.0.0.1
-- Tiempo de generación: 27-05-2024 a las 16:21:26
-- Versión del servidor: 10.4.18-MariaDB
-- Versión de PHP: 8.0.5

SET SQL_MODE = "NO_AUTO_VALUE_ON_ZERO";
START TRANSACTION;
SET time_zone = "+00:00";


/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!40101 SET NAMES utf8mb4 */;

--
-- Base de datos: `apiinmobiliaria`
--
CREATE DATABASE IF NOT EXISTS `apiinmobiliaria` DEFAULT CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci;
USE `apiinmobiliaria`;

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `contratos`
--

CREATE TABLE `contratos` (
  `Id` int(11) NOT NULL,
  `InmuebleId` int(11) NOT NULL,
  `InquilinoId` int(11) NOT NULL,
  `Precio` decimal(10,2) NOT NULL,
  `FechaInicio` date NOT NULL,
  `FechaFin` date NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

--
-- Volcado de datos para la tabla `contratos`
--

INSERT INTO `contratos` (`Id`, `InmuebleId`, `InquilinoId`, `Precio`, `FechaInicio`, `FechaFin`) VALUES
(1, 1, 1, '5900.00', '2024-01-01', '2027-01-01'),
(2, 1, 1, '1200.00', '2016-08-14', '2023-12-31'),
(3, 2, 2, '800.00', '2010-01-01', '2019-12-31'),
(4, 2, 2, '4700.00', '2020-01-01', '2029-12-31'),
(5, 4, 1, '12000.00', '2022-01-01', '2026-12-31'),
(6, 4, 2, '12000.00', '2027-01-01', '2029-12-31');

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `inmuebles`
--

CREATE TABLE `inmuebles` (
  `Id` int(11) NOT NULL,
  `PropietarioId` int(11) NOT NULL,
  `Direccion` varchar(100) NOT NULL,
  `Ambientes` int(11) NOT NULL,
  `TipoId` int(11) NOT NULL,
  `UsoId` int(11) NOT NULL,
  `Precio` decimal(10,2) NOT NULL,
  `ImagenUrl` varchar(2000) DEFAULT NULL,
  `Estado` tinyint(1) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

--
-- Volcado de datos para la tabla `inmuebles`
--

INSERT INTO `inmuebles` (`Id`, `PropietarioId`, `Direccion`, `Ambientes`, `TipoId`, `UsoId`, `Precio`, `ImagenUrl`, `Estado`) VALUES
(1, 6, 'Barrio Cerro de la Cruz Manzana 265 Casa 10', 3, 1, 1, '59000.00', 'uploads/inmueble_6_1.jpg', 1),
(2, 6, 'Carranza 1129', 3, 1, 1, '47000.00', 'uploads/inmueble_6_2.jpg', 1),
(3, 6, 'San Luis 1234', 1, 4, 2, '120000.00', 'uploads/inmueble_6_3.jpg', 1),
(4, 6, 'Presidente Perón 4900', 7, 3, 2, '375000.92', 'uploads/inmueble_6_4.jpg', 0);

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `inquilinos`
--

CREATE TABLE `inquilinos` (
  `Id` int(11) NOT NULL,
  `DNI` varchar(12) NOT NULL,
  `Apellido` varchar(100) NOT NULL,
  `Nombre` varchar(100) NOT NULL,
  `Email` varchar(100) NOT NULL,
  `Telefono` varchar(20) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

--
-- Volcado de datos para la tabla `inquilinos`
--

INSERT INTO `inquilinos` (`Id`, `DNI`, `Apellido`, `Nombre`, `Email`, `Telefono`) VALUES
(1, '37716731', 'Cruceño', 'Federico Ivan', 'fedeicru@gmail.com', '2657312733'),
(2, '12600842', 'Orsomarso', 'Dora Nelida', 'doranel50@hotmail.com', '1163213910');

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `pagos`
--

CREATE TABLE `pagos` (
  `Id` int(11) NOT NULL,
  `ContratoId` int(11) NOT NULL,
  `NumeroDePago` int(11) NOT NULL,
  `Fecha` date NOT NULL,
  `Monto` decimal(10,2) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

--
-- Volcado de datos para la tabla `pagos`
--

INSERT INTO `pagos` (`Id`, `ContratoId`, `NumeroDePago`, `Fecha`, `Monto`) VALUES
(1, 1, 1, '2024-01-01', '5900.00'),
(2, 1, 2, '2024-02-01', '5900.00'),
(3, 1, 3, '2024-03-01', '5900.00'),
(4, 1, 4, '2024-04-01', '5900.00'),
(5, 1, 5, '2024-05-01', '5900.00'),
(6, 5, 1, '2022-01-01', '12000.00'),
(7, 5, 2, '2022-02-01', '12000.00'),
(8, 5, 3, '2022-03-01', '12000.00'),
(9, 5, 4, '2022-04-01', '12000.00'),
(10, 5, 5, '2022-05-01', '12000.00'),
(11, 5, 6, '2022-06-01', '12000.00'),
(12, 5, 7, '2022-07-01', '12000.00'),
(13, 5, 8, '2022-08-01', '12000.00'),
(14, 5, 9, '2022-09-01', '12000.00'),
(15, 5, 10, '2022-10-01', '12000.00'),
(16, 5, 11, '2022-11-01', '12000.00'),
(17, 5, 12, '2022-12-01', '12000.00'),
(18, 5, 13, '2023-01-01', '12000.00'),
(19, 5, 14, '2023-02-01', '12000.00'),
(20, 5, 15, '2023-03-01', '12000.00'),
(21, 5, 16, '2023-04-01', '12000.00'),
(22, 5, 17, '2023-05-01', '12000.00'),
(23, 5, 18, '2023-06-01', '12000.00'),
(24, 5, 19, '2023-07-01', '12000.00'),
(25, 5, 20, '2023-08-01', '12000.00'),
(26, 5, 21, '2023-09-01', '12000.00'),
(27, 5, 22, '2023-10-01', '12000.00'),
(28, 5, 23, '2023-11-01', '12000.00'),
(29, 5, 24, '2023-12-01', '12000.00'),
(30, 5, 25, '2024-01-01', '12000.00'),
(31, 5, 26, '2024-02-01', '12000.00'),
(32, 5, 27, '2024-03-01', '12000.00'),
(33, 5, 28, '2024-04-01', '12000.00'),
(34, 5, 29, '2024-05-01', '12000.00'),
(35, 4, 1, '2020-01-01', '4700.00'),
(36, 4, 2, '2020-02-01', '4700.00'),
(37, 4, 3, '2020-03-01', '4700.00'),
(38, 4, 4, '2020-04-01', '4700.00'),
(39, 4, 5, '2020-05-01', '4700.00'),
(40, 4, 6, '2020-06-01', '4700.00'),
(41, 4, 7, '2020-07-01', '4700.00'),
(42, 4, 8, '2020-08-01', '4700.00'),
(43, 4, 9, '2020-09-01', '4700.00'),
(44, 4, 11, '2020-10-01', '4700.00'),
(45, 4, 12, '2020-11-01', '4700.00'),
(46, 4, 13, '2020-12-01', '4700.00'),
(47, 4, 14, '2021-01-01', '4700.00'),
(48, 4, 15, '2021-02-01', '4700.00'),
(49, 4, 16, '2021-03-01', '4700.00'),
(50, 4, 17, '2021-04-01', '4700.00'),
(51, 4, 18, '2021-05-01', '4700.00'),
(52, 4, 19, '2021-06-01', '4700.00'),
(53, 4, 20, '2021-07-01', '4700.00'),
(54, 4, 21, '2021-08-01', '4700.00'),
(55, 4, 22, '2021-09-01', '4700.00'),
(56, 4, 23, '2021-10-01', '4700.00'),
(57, 4, 24, '2021-11-01', '4700.00'),
(58, 4, 25, '2021-12-01', '4700.00'),
(59, 4, 26, '2022-01-01', '4700.00'),
(60, 4, 27, '2022-02-01', '4700.00'),
(61, 4, 28, '2022-03-01', '4700.00'),
(62, 4, 29, '2022-04-01', '4700.00'),
(63, 4, 30, '2022-05-01', '4700.00'),
(64, 4, 31, '2022-06-01', '4700.00'),
(65, 4, 32, '2022-07-01', '4700.00'),
(66, 4, 33, '2022-08-01', '4700.00'),
(67, 4, 34, '2022-09-01', '4700.00'),
(68, 4, 35, '2022-10-01', '4700.00'),
(69, 4, 36, '2022-11-01', '4700.00'),
(70, 4, 37, '2022-12-01', '4700.00'),
(71, 4, 38, '2023-01-01', '4700.00'),
(72, 4, 39, '2023-02-01', '4700.00'),
(73, 4, 40, '2023-03-01', '4700.00'),
(74, 4, 41, '2023-04-01', '4700.00'),
(75, 4, 42, '2023-05-01', '4700.00'),
(76, 4, 43, '2023-06-01', '4700.00'),
(77, 4, 44, '2023-07-01', '4700.00'),
(78, 4, 45, '2023-08-01', '4700.00'),
(79, 4, 46, '2023-09-01', '4700.00'),
(80, 4, 47, '2023-10-01', '4700.00'),
(81, 4, 48, '2023-11-01', '4700.00'),
(82, 4, 49, '2023-12-01', '4700.00'),
(83, 4, 50, '2024-01-01', '4700.00'),
(84, 4, 51, '2024-02-01', '4700.00'),
(85, 4, 52, '2024-03-01', '4700.00'),
(86, 4, 53, '2024-04-01', '4700.00'),
(87, 4, 54, '2024-05-01', '4700.00');

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `propietarios`
--

CREATE TABLE `propietarios` (
  `Id` int(11) NOT NULL,
  `DNI` varchar(12) NOT NULL,
  `Apellido` varchar(50) NOT NULL,
  `Nombre` varchar(50) NOT NULL,
  `Telefono` varchar(50) NOT NULL,
  `Email` varchar(100) NOT NULL,
  `Password` varchar(255) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

--
-- Volcado de datos para la tabla `propietarios`
--

INSERT INTO `propietarios` (`Id`, `DNI`, `Apellido`, `Nombre`, `Telefono`, `Email`, `Password`) VALUES
(1, '12345678', 'Luzza', 'Mariano', '121221224', 'mluzza@gmail.com', '3A0G2+zJ3luLnlC44+Xe5HGw/9RWJNoyF2XZACvev20='),
(2, '27013989', 'Baigorria', 'Monica Patricia', '2657123455', 'patobaigorria@gmail.com', '3A0G2+zJ3luLnlC44+Xe5HGw/9RWJNoyF2XZACvev20='),
(6, '34229421', 'Orsomarso', 'Ezequiel', '1132185230', 'diazezequiel777@gmail.com', '3A0G2+zJ3luLnlC44+Xe5HGw/9RWJNoyF2XZACvev20='),
(7, '37716731', 'Cruceño', 'Federico', '2657312733', 'a', '3NhEe7xWmOI/rcoD1E87QOXvp/dxtXzdcKYcAKt41tM=');

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `tipodeinmueble`
--

CREATE TABLE `tipodeinmueble` (
  `Id` int(11) NOT NULL,
  `nombre` varchar(30) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

--
-- Volcado de datos para la tabla `tipodeinmueble`
--

INSERT INTO `tipodeinmueble` (`Id`, `nombre`) VALUES
(1, 'Casa'),
(2, 'Departamento'),
(3, 'Local'),
(4, 'Depósito');

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `usodeinmueble`
--

CREATE TABLE `usodeinmueble` (
  `Id` int(11) NOT NULL,
  `Nombre` varchar(30) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

--
-- Volcado de datos para la tabla `usodeinmueble`
--

INSERT INTO `usodeinmueble` (`Id`, `Nombre`) VALUES
(1, 'Residencial'),
(2, 'Comercial');

--
-- Índices para tablas volcadas
--

--
-- Indices de la tabla `contratos`
--
ALTER TABLE `contratos`
  ADD PRIMARY KEY (`Id`),
  ADD KEY `InmuebleId` (`InmuebleId`),
  ADD KEY `InquilinoId` (`InquilinoId`);

--
-- Indices de la tabla `inmuebles`
--
ALTER TABLE `inmuebles`
  ADD PRIMARY KEY (`Id`),
  ADD KEY `PropietarioId` (`PropietarioId`),
  ADD KEY `TipoId` (`TipoId`),
  ADD KEY `UsoId` (`UsoId`);

--
-- Indices de la tabla `inquilinos`
--
ALTER TABLE `inquilinos`
  ADD PRIMARY KEY (`Id`);

--
-- Indices de la tabla `pagos`
--
ALTER TABLE `pagos`
  ADD PRIMARY KEY (`Id`),
  ADD KEY `ContratoId` (`ContratoId`);

--
-- Indices de la tabla `propietarios`
--
ALTER TABLE `propietarios`
  ADD PRIMARY KEY (`Id`);

--
-- Indices de la tabla `tipodeinmueble`
--
ALTER TABLE `tipodeinmueble`
  ADD PRIMARY KEY (`Id`);

--
-- Indices de la tabla `usodeinmueble`
--
ALTER TABLE `usodeinmueble`
  ADD PRIMARY KEY (`Id`);

--
-- AUTO_INCREMENT de las tablas volcadas
--

--
-- AUTO_INCREMENT de la tabla `contratos`
--
ALTER TABLE `contratos`
  MODIFY `Id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=7;

--
-- AUTO_INCREMENT de la tabla `inmuebles`
--
ALTER TABLE `inmuebles`
  MODIFY `Id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=5;

--
-- AUTO_INCREMENT de la tabla `inquilinos`
--
ALTER TABLE `inquilinos`
  MODIFY `Id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=3;

--
-- AUTO_INCREMENT de la tabla `pagos`
--
ALTER TABLE `pagos`
  MODIFY `Id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=88;

--
-- AUTO_INCREMENT de la tabla `propietarios`
--
ALTER TABLE `propietarios`
  MODIFY `Id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=8;

--
-- AUTO_INCREMENT de la tabla `tipodeinmueble`
--
ALTER TABLE `tipodeinmueble`
  MODIFY `Id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=5;

--
-- AUTO_INCREMENT de la tabla `usodeinmueble`
--
ALTER TABLE `usodeinmueble`
  MODIFY `Id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=3;

--
-- Restricciones para tablas volcadas
--

--
-- Filtros para la tabla `contratos`
--
ALTER TABLE `contratos`
  ADD CONSTRAINT `contratos_ibfk_1` FOREIGN KEY (`InmuebleId`) REFERENCES `inmuebles` (`Id`),
  ADD CONSTRAINT `contratos_ibfk_2` FOREIGN KEY (`InquilinoId`) REFERENCES `inquilinos` (`Id`);

--
-- Filtros para la tabla `inmuebles`
--
ALTER TABLE `inmuebles`
  ADD CONSTRAINT `inmuebles_ibfk_1` FOREIGN KEY (`PropietarioId`) REFERENCES `propietarios` (`Id`),
  ADD CONSTRAINT `inmuebles_ibfk_2` FOREIGN KEY (`TipoId`) REFERENCES `tipodeinmueble` (`Id`),
  ADD CONSTRAINT `inmuebles_ibfk_3` FOREIGN KEY (`UsoId`) REFERENCES `usodeinmueble` (`Id`);

--
-- Filtros para la tabla `pagos`
--
ALTER TABLE `pagos`
  ADD CONSTRAINT `pagos_ibfk_1` FOREIGN KEY (`ContratoId`) REFERENCES `contratos` (`Id`);
COMMIT;

/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
