-- sp_CheckIn_Operations.sql
DELIMITER //

-- Obtener reservaciones pendientes para check-in
CREATE PROCEDURE sp_GetPendingReservationsForCheckIn()
BEGIN
    SELECT 
        r.id,
        CONCAT(c.nombre, ' ', c.apellido) AS cliente,
        h.numero AS habitacion,
        th.nombre AS tipo_habitacion,
        r.fecha_entrada,
        r.fecha_salida,
        DATEDIFF(r.fecha_salida, r.fecha_entrada) AS noches,
        r.precio_total,
        r.adultos,
        r.ninos,
        r.notas AS notas_reserva,
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
        usuarios u ON r.usuario_id = u.id
    WHERE 
        r.estado_id = 1 -- Pendiente
        AND r.fecha_entrada <= DATE_ADD(CURRENT_DATE, INTERVAL 1 DAY) -- Reservaciones para hoy o próximas
    ORDER BY 
        r.fecha_entrada;
END //

-- Obtener detalles completos de una reserva para check-in
CREATE PROCEDURE sp_GetReservationDetailsForCheckIn(IN p_reserva_id INT)
BEGIN
    -- Información de la reserva
    SELECT 
        r.*,
        CONCAT(c.nombre, ' ', c.apellido) AS cliente_nombre,
        c.tipo_documento,
        c.documento_identidad,
        c.telefono,
        c.correo,
        h.numero AS habitacion_numero,
        th.nombre AS tipo_habitacion,
        th.capacidad,
        th.precio_base,
        er.nombre AS estado_nombre,
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
        r.id = p_reserva_id;
    
    -- Servicios adicionales
    SELECT 
        sr.id,
        sa.nombre AS servicio,
        sr.cantidad,
        sr.fecha,
        sr.precio_unitario,
        (sr.cantidad * sr.precio_unitario) AS total,
        sr.notas
    FROM 
        servicios_reserva sr
    JOIN 
        servicios_adicionales sa ON sr.servicio_id = sa.id
    WHERE 
        sr.reserva_id = p_reserva_id
    ORDER BY 
        sr.fecha;
END //

-- Realizar check-in de una reserva
CREATE PROCEDURE sp_PerformCheckIn(
    IN p_reserva_id INT,
    IN p_usuario_id INT,
    IN p_metodo_pago ENUM('Efectivo', 'Tarjeta', 'Transferencia', 'Otro'),
    IN p_documentos_recibidos BOOLEAN,
    IN p_deposito_seguridad DECIMAL(10,2),
    IN p_observaciones TEXT
)
BEGIN
    DECLARE v_habitacion_id INT;
    
    -- Obtener ID de la habitación
    SELECT habitacion_id INTO v_habitacion_id FROM reservas WHERE id = p_reserva_id;
    
    -- Registrar el check-in
    INSERT INTO checkins (
        reserva_id,
        usuario_id,
        metodo_pago,
        documentos_recibidos,
        deposito_seguridad,
        observaciones
    ) VALUES (
        p_reserva_id,
        p_usuario_id,
        p_metodo_pago,
        p_documentos_recibidos,
        p_deposito_seguridad,
        p_observaciones
    );
    
    -- Actualizar estado de la reserva a Check-In (2)
    UPDATE reservas SET estado_id = 2 WHERE id = p_reserva_id;
    
    -- Actualizar estado de la habitación a Ocupada
    UPDATE habitaciones SET estado = 'Ocupada' WHERE id = v_habitacion_id;
    
    SELECT LAST_INSERT_ID() AS checkin_id;
END //

-- Obtener información de un check-in
CREATE PROCEDURE sp_GetCheckInDetails(IN p_checkin_id INT)
BEGIN
    SELECT 
        ci.*,
        r.id AS reserva_id,
        CONCAT(c.nombre, ' ', c.apellido) AS cliente,
        h.numero AS habitacion,
        th.nombre AS tipo_habitacion,
        u.nombre_completo AS recepcionista
    FROM 
        checkins ci
    JOIN 
        reservas r ON ci.reserva_id = r.id
    JOIN 
        clientes c ON r.cliente_id = c.id
    JOIN 
        habitaciones h ON r.habitacion_id = h.id
    JOIN 
        tipos_habitacion th ON h.tipo_id = th.id
    JOIN 
        usuarios u ON ci.usuario_id = u.id
    WHERE 
        ci.id = p_checkin_id;
END //

-- Actualizar información de un check-in
CREATE PROCEDURE sp_UpdateCheckIn(
    IN p_checkin_id INT,
    IN p_metodo_pago ENUM('Efectivo', 'Tarjeta', 'Transferencia', 'Otro'),
    IN p_documentos_recibidos BOOLEAN,
    IN p_deposito_seguridad DECIMAL(10,2),
    IN p_observaciones TEXT
)
BEGIN
    UPDATE checkins SET
        metodo_pago = p_metodo_pago,
        documentos_recibidos = p_documentos_recibidos,
        deposito_seguridad = p_deposito_seguridad,
        observaciones = p_observaciones
    WHERE id = p_checkin_id;
END //

DELIMITER ;