-- Crear base de datos
CREATE DATABASE IF NOT EXISTS ped_hotel_reservas;
USE ped_hotel_reservas;

-- Tabla de roles (Admin, Recepcionista, etc.)
CREATE TABLE roles (
  id INT AUTO_INCREMENT PRIMARY KEY,
  nombre_rol VARCHAR(50) NOT NULL UNIQUE,
  descripcion VARCHAR(255)
);

-- Insertar roles iniciales
INSERT INTO roles (nombre_rol, descripcion) VALUES
('Administrador', 'Acceso completo al sistema'),
('Recepcionista', 'Puede gestionar reservas y clientes'),
('Limpieza', 'Puede ver estado de habitaciones');

-- Tabla de usuarios con relación a roles
CREATE TABLE usuarios (
  id INT AUTO_INCREMENT PRIMARY KEY,
  nombre_usuario VARCHAR(50) NOT NULL UNIQUE,
  contrasena VARCHAR(255) NOT NULL,
  nombre_completo VARCHAR(100),
  rol_id INT NOT NULL,
  activo BOOLEAN DEFAULT TRUE,
  fecha_creacion DATETIME DEFAULT CURRENT_TIMESTAMP,
  FOREIGN KEY (rol_id) REFERENCES roles(id)
);

-- Insertar usuarios iniciales (contraseñas igual al nombre de usuario)
INSERT INTO usuarios (nombre_usuario, contrasena, nombre_completo, rol_id) VALUES
('admin', 'admin', 'Administrador Principal', 1),
('recepcion', 'recepcion', 'Recepcionista Principal', 2),
('limpieza', 'limpieza', 'Personal de Limpieza', 3);

-- Tabla de tipos de habitación
CREATE TABLE tipos_habitacion (
  id INT AUTO_INCREMENT PRIMARY KEY,
  nombre VARCHAR(50) NOT NULL UNIQUE,
  descripcion VARCHAR(255),
  capacidad INT NOT NULL,
  precio_base DECIMAL(10,2) NOT NULL
);

-- Insertar tipos de habitación
INSERT INTO tipos_habitacion (nombre, descripcion, capacidad, precio_base) VALUES
('Individual', 'Habitación individual con cama sencilla', 1, 80.00),
('Doble', 'Habitación con cama doble o dos camas individuales', 2, 120.00),
('Suite', 'Suite con sala independiente y amenities', 2, 200.00),
('Familiar', 'Habitación grande para familias', 4, 180.00);

-- Tabla de habitaciones
CREATE TABLE habitaciones (
  id INT AUTO_INCREMENT PRIMARY KEY,
  numero VARCHAR(10) NOT NULL UNIQUE,
  tipo_id INT NOT NULL,
  piso INT,
  estado ENUM('Disponible', 'Ocupada', 'Mantenimiento', 'Limpieza') DEFAULT 'Disponible',
  ultima_limpieza DATETIME,
  notas TEXT,
  FOREIGN KEY (tipo_id) REFERENCES tipos_habitacion(id)
);

-- Insertar habitaciones de ejemplo
INSERT INTO habitaciones (numero, tipo_id, piso, estado) VALUES
('101', 1, 1, 'Disponible'),
('102', 1, 1, 'Disponible'),
('201', 2, 2, 'Disponible'),
('202', 2, 2, 'Disponible'),
('301', 3, 3, 'Disponible'),
('401', 4, 4, 'Disponible');

-- Tabla de clientes
CREATE TABLE clientes (
  id INT AUTO_INCREMENT PRIMARY KEY,
  nombre VARCHAR(100) NOT NULL,
  apellido VARCHAR(100),
  tipo_documento ENUM('DUI', 'Pasaporte', 'Carnet Extranjería', 'Otro') DEFAULT 'DUI',
  documento_identidad VARCHAR(20) NOT NULL,
  telefono VARCHAR(20),
  correo VARCHAR(100),
  direccion TEXT,
  pais VARCHAR(50),
  fecha_registro DATETIME DEFAULT CURRENT_TIMESTAMP,
  notas TEXT,
  UNIQUE KEY (tipo_documento, documento_identidad)
);

-- Insertar clientes de ejemplo
INSERT INTO clientes (nombre, apellido, tipo_documento, documento_identidad, telefono, correo) VALUES
('Juan', 'Pérez', 'DUI', '12345678', '555-1234', 'juan.perez@example.com'),
('María', 'García', 'DUI', '87654321', '555-5678', 'maria.garcia@example.com'),
('Carlos', 'López', 'Pasaporte', 'PA123456', '555-9012', 'carlos.lopez@example.com');

-- Tabla de estados de reserva
CREATE TABLE estados_reserva (
  id INT AUTO_INCREMENT PRIMARY KEY,
  nombre VARCHAR(50) NOT NULL UNIQUE,
  descripcion VARCHAR(255)
);

-- Insertar estados de reserva
INSERT INTO estados_reserva (nombre, descripcion) VALUES
('Pendiente', 'Reserva confirmada pero no ha comenzado'),
('Check-In', 'El huésped ha ingresado a la habitación'),
('Check-Out', 'El huésped ha salido de la habitación'),
('Cancelada', 'Reserva cancelada'),
('No-Show', 'El huésped no se presentó');

-- Tabla de reservas
CREATE TABLE reservas (
  id INT AUTO_INCREMENT PRIMARY KEY,
  cliente_id INT NOT NULL,
  habitacion_id INT NOT NULL,
  usuario_id INT NOT NULL,
  estado_id INT NOT NULL DEFAULT 1,
  fecha_entrada DATE NOT NULL,
  fecha_salida DATE NOT NULL,
  adultos INT NOT NULL DEFAULT 1,
  ninos INT NOT NULL DEFAULT 0,
  precio_total DECIMAL(10,2) NOT NULL,
  notas TEXT,
  fecha_creacion DATETIME DEFAULT CURRENT_TIMESTAMP,
  fecha_actualizacion DATETIME ON UPDATE CURRENT_TIMESTAMP,
  FOREIGN KEY (cliente_id) REFERENCES clientes(id),
  FOREIGN KEY (habitacion_id) REFERENCES habitaciones(id),
  FOREIGN KEY (usuario_id) REFERENCES usuarios(id),
  FOREIGN KEY (estado_id) REFERENCES estados_reserva(id),
  CHECK (fecha_salida > fecha_entrada)
);

-- Tabla de check-ins
CREATE TABLE checkins (
  id INT AUTO_INCREMENT PRIMARY KEY,
  reserva_id INT NOT NULL,
  usuario_id INT NOT NULL,
  fecha_hora DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
  metodo_pago ENUM('Efectivo', 'Tarjeta', 'Transferencia', 'Otro') NOT NULL,
  documentos_recibidos BOOLEAN DEFAULT FALSE,
  deposito_seguridad DECIMAL(10,2),
  observaciones TEXT,
  FOREIGN KEY (reserva_id) REFERENCES reservas(id),
  FOREIGN KEY (usuario_id) REFERENCES usuarios(id)
);

-- Tabla de check-outs
CREATE TABLE checkouts (
  id INT AUTO_INCREMENT PRIMARY KEY,
  reserva_id INT NOT NULL,
  usuario_id INT NOT NULL,
  fecha_hora DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
  estado_habitacion ENUM('Excelente', 'Bueno', 'Daños menores', 'Daños graves') DEFAULT 'Bueno',
  cobros_adicionales DECIMAL(10,2) DEFAULT 0,
  devolucion_deposito DECIMAL(10,2),
  observaciones TEXT,
  FOREIGN KEY (reserva_id) REFERENCES reservas(id),
  FOREIGN KEY (usuario_id) REFERENCES usuarios(id)
);

-- Tabla de servicios adicionales
CREATE TABLE servicios_adicionales (
  id INT AUTO_INCREMENT PRIMARY KEY,
  nombre VARCHAR(100) NOT NULL,
  descripcion TEXT,
  precio DECIMAL(10,2) NOT NULL,
  activo BOOLEAN DEFAULT TRUE
);

-- Insertar servicios adicionales
INSERT INTO servicios_adicionales (nombre, descripcion, precio) VALUES
('Desayuno', 'Desayuno buffet', 15.00),
('Parking', 'Estacionamiento por día', 10.00),
('Late Check-Out', 'Salida hasta las 3pm', 20.00),
('Lavandería', 'Servicio de lavandería', 25.00);

-- Tabla de servicios por reserva
CREATE TABLE servicios_reserva (
  id INT AUTO_INCREMENT PRIMARY KEY,
  reserva_id INT NOT NULL,
  servicio_id INT NOT NULL,
  cantidad INT NOT NULL DEFAULT 1,
  fecha DATE NOT NULL,
  precio_unitario DECIMAL(10,2) NOT NULL,
  notas TEXT,
  FOREIGN KEY (reserva_id) REFERENCES reservas(id),
  FOREIGN KEY (servicio_id) REFERENCES servicios_adicionales(id)
);

-- Vista para ver disponibilidad de habitaciones
CREATE VIEW vista_disponibilidad AS
SELECT 
  h.id,
  h.numero,
  th.nombre AS tipo,
  th.capacidad,
  th.precio_base AS precio,
  h.piso,
  h.estado,
  COUNT(r.id) AS reservas_activas
FROM 
  habitaciones h
JOIN 
  tipos_habitacion th ON h.tipo_id = th.id
LEFT JOIN 
  reservas r ON h.id = r.habitacion_id 
  AND r.estado_id IN (1, 2) -- Pendiente o Check-In
  AND CURRENT_DATE BETWEEN r.fecha_entrada AND r.fecha_salida
GROUP BY 
  h.id;

-- Vista para ver reservas activas
CREATE VIEW vista_reservas_activas AS
SELECT 
  r.id,
  CONCAT(c.nombre, ' ', c.apellido) AS cliente,
  h.numero AS habitacion,
  th.nombre AS tipo_habitacion,
  r.fecha_entrada,
  r.fecha_salida,
  DATEDIFF(r.fecha_salida, r.fecha_entrada) AS noches,
  r.precio_total,
  er.nombre AS estado,
  u.nombre_completo AS recepcionista
FROM 
  reservas r
JOIN 
  clientes c ON r.cliente_id = c.id
JOIN 
  habitaciones h ON r.habitacion_id = h.id
JOIN 
  tipos_habitacion th ON h.tipo_id = th.id
JOIN 
  estados_reserva er ON r.estado_id = er.id
JOIN 
  usuarios u ON r.usuario_id = u.id
WHERE 
  r.estado_id IN (1, 2) -- Pendiente o Check-In
ORDER BY 
  r.fecha_entrada;