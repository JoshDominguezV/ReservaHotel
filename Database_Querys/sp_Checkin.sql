-- Procedimiento para obtener reservas pendientes de check-in
use ped_hotel_reservas;
DELIMITER //
CREATE PROCEDURE sp_GetPendingCheckIns()
BEGIN
    SELECT 
        r.id AS reserva_id,
        CONCAT(c.nombre, ' ', c.apellido) AS cliente,
        h.numero AS habitacion,
        th.nombre AS tipo_habitacion,
        r.fecha_entrada,
        r.fecha_salida,
        DATEDIFF(r.fecha_salida, r.fecha_entrada) AS noches,
        r.precio_total,
        r.adultos,
        r.ninos
    FROM 
        reservas r
    JOIN 
        clientes c ON r.cliente_id = c.id
    JOIN 
        habitaciones h ON r.habitacion_id = h.id
    JOIN 
        tipos_habitacion th ON h.tipo_id = th.id
    WHERE 
        r.estado_id = 1 -- Pendiente
        AND r.fecha_entrada <= CURRENT_DATE
    ORDER BY 
        r.fecha_entrada;
END //
DELIMITER ;

-- Procedimiento para realizar check-in
DELIMITER //
CREATE PROCEDURE sp_CheckInReserva(
    IN p_reserva_id INT,
    IN p_usuario_id INT,
    IN p_metodo_pago VARCHAR(50),
    IN p_deposito_seguridad DECIMAL(10,2),
    IN p_observaciones TEXT
)
BEGIN
    DECLARE EXIT HANDLER FOR SQLEXCEPTION
    BEGIN
        ROLLBACK;
        RESIGNAL;
    END;
    
    START TRANSACTION;
    
    -- Actualizar estado de la reserva a Check-In
    UPDATE reservas 
    SET estado_id = (SELECT id FROM estados_reserva WHERE nombre = 'Check-In')
    WHERE id = p_reserva_id;
    
    -- Actualizar estado de la habitación a Ocupada
    UPDATE habitaciones h
    JOIN reservas r ON h.id = r.habitacion_id
    SET h.estado = 'Ocupada'
    WHERE r.id = p_reserva_id;
    
    -- Registrar el check-in
    INSERT INTO checkins (
        reserva_id, 
        usuario_id, 
        metodo_pago, 
        deposito_seguridad, 
        observaciones
    ) VALUES (
        p_reserva_id, 
        p_usuario_id, 
        p_metodo_pago, 
        p_deposito_seguridad, 
        p_observaciones
    );
    
    COMMIT;
END //
DELIMITER ;

-- Procedimiento para obtener información detallada de una reserva
DELIMITER //
CREATE PROCEDURE sp_GetReservaDetails(IN p_reserva_id INT)
BEGIN
    SELECT 
        r.id,
        CONCAT(c.nombre, ' ', c.apellido) AS cliente,
        c.tipo_documento,
        c.documento_identidad,
        c.telefono,
        c.correo,
        h.numero AS habitacion,
        th.nombre AS tipo_habitacion,
        th.precio_base,
        r.fecha_entrada,
        r.fecha_salida,
        DATEDIFF(r.fecha_salida, r.fecha_entrada) AS noches,
        r.adultos,
        r.ninos,
        r.precio_total,
        er.nombre AS estado_reserva
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
    WHERE 
        r.id = p_reserva_id;
END //
DELIMITER ;

-- Procedimiento para obtener servicios adicionales disponibles
DELIMITER //
CREATE PROCEDURE sp_GetAvailableServices()
BEGIN
    SELECT 
        id, 
        nombre, 
        descripcion, 
        precio 
    FROM 
        servicios_adicionales 
    WHERE 
        activo = TRUE;
END //
DELIMITER ;

-- Procedimiento para agregar servicios a una reserva
DELIMITER //
CREATE PROCEDURE sp_AddServiceToReserva(
    IN p_reserva_id INT,
    IN p_servicio_id INT,
    IN p_cantidad INT,
    IN p_fecha DATE
)
BEGIN
    DECLARE v_precio DECIMAL(10,2);
    
    -- Obtener precio actual del servicio
    SELECT precio INTO v_precio 
    FROM servicios_adicionales 
    WHERE id = p_servicio_id;
    
    -- Insertar servicio en la reserva
    INSERT INTO servicios_reserva (
        reserva_id, 
        servicio_id, 
        cantidad, 
        fecha, 
        precio_unitario
    ) VALUES (
        p_reserva_id, 
        p_servicio_id, 
        p_cantidad, 
        p_fecha, 
        v_precio
    );
    
    -- Actualizar precio total de la reserva
    UPDATE reservas r
    SET r.precio_total = r.precio_total + (v_precio * p_cantidad)
    WHERE r.id = p_reserva_id;
END //
DELIMITER ;

-- Procedimiento para obtener servicios de una reserva
DELIMITER //
CREATE PROCEDURE sp_GetReservaServices(IN p_reserva_id INT)
BEGIN
    SELECT 
        sr.id,
        sa.nombre AS servicio,
        sr.cantidad,
        sr.precio_unitario,
        (sr.cantidad * sr.precio_unitario) AS subtotal,
        sr.fecha
    FROM 
        servicios_reserva sr
    JOIN 
        servicios_adicionales sa ON sr.servicio_id = sa.id
    WHERE 
        sr.reserva_id = p_reserva_id;
END //
DELIMITER ;