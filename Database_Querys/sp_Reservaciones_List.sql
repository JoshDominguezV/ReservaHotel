DELIMITER //

-- Obtener todas las reservaciones con información completa
CREATE PROCEDURE sp_GetAllReservations()
BEGIN
    SELECT 
        r.id,
        CONCAT(c.nombre, ' ', c.apellido) AS cliente,
        h.numero AS habitacion,
        er.nombre AS estado,
        r.fecha_entrada,
        r.fecha_salida,
        r.precio_total,
        u.nombre_completo AS recepcionista
    FROM 
        reservas r
    JOIN 
        clientes c ON r.cliente_id = c.id
    JOIN 
        habitaciones h ON r.habitacion_id = h.id
    JOIN 
        estados_reserva er ON r.estado_id = er.id
    JOIN 
        usuarios u ON r.usuario_id = u.id
    ORDER BY 
        r.fecha_entrada DESC;
END //

-- Buscar reservaciones por término de búsqueda
CREATE PROCEDURE sp_SearchReservations(IN p_busqueda VARCHAR(100))
BEGIN
    SELECT 
        r.id,
        CONCAT(c.nombre, ' ', c.apellido) AS cliente,
        h.numero AS habitacion,
        er.nombre AS estado,
        r.fecha_entrada,
        r.fecha_salida,
        r.precio_total,
        u.nombre_completo AS recepcionista
    FROM 
        reservas r
    JOIN 
        clientes c ON r.cliente_id = c.id
    JOIN 
        habitaciones h ON r.habitacion_id = h.id
    JOIN 
        estados_reserva er ON r.estado_id = er.id
    JOIN 
        usuarios u ON r.usuario_id = u.id
    WHERE 
        c.nombre LIKE CONCAT('%', p_busqueda, '%') OR
        c.apellido LIKE CONCAT('%', p_busqueda, '%') OR
        h.numero LIKE CONCAT('%', p_busqueda, '%') OR
        u.nombre_completo LIKE CONCAT('%', p_busqueda, '%')
    ORDER BY 
        r.fecha_entrada DESC;
END //

-- Filtrar reservaciones por estado
CREATE PROCEDURE sp_FilterReservationsByStatus(IN p_estado VARCHAR(50))
BEGIN
    SELECT 
        r.id,
        CONCAT(c.nombre, ' ', c.apellido) AS cliente,
        h.numero AS habitacion,
        er.nombre AS estado,
        r.fecha_entrada,
        r.fecha_salida,
        r.precio_total,
        u.nombre_completo AS recepcionista
    FROM 
        reservas r
    JOIN 
        clientes c ON r.cliente_id = c.id
    JOIN 
        habitaciones h ON r.habitacion_id = h.id
    JOIN 
        estados_reserva er ON r.estado_id = er.id
    JOIN 
        usuarios u ON r.usuario_id = u.id
    WHERE 
        er.nombre = p_estado
    ORDER BY 
        r.fecha_entrada DESC;
END //
-- Obtener detalles de una reserva específica
CREATE PROCEDURE sp_GetReservationDetails(IN p_reserva_id INT)
BEGIN
    SELECT 
        r.*,
        CONCAT(c.nombre, ' ', c.apellido) AS cliente_nombre,
        h.numero AS habitacion_numero,
        th.nombre AS tipo_habitacion,
        er.nombre AS estado_nombre
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

-- Eliminar una reservación
CREATE PROCEDURE sp_DeleteReservation(IN p_id INT)
BEGIN
    -- Primero eliminar servicios asociados
    DELETE FROM servicios_reserva WHERE reserva_id = p_id;
    
    -- Luego eliminar la reserva
    DELETE FROM reservas WHERE id = p_id;
END //
CREATE PROCEDURE sp_UpdateReservation(
    IN p_reserva_id INT,
    IN p_cliente_id INT,
    IN p_habitacion_id INT,
    IN p_fecha_entrada DATE,
    IN p_fecha_salida DATE,
    IN p_adultos INT,
    IN p_ninos INT,
    IN p_precio_total DECIMAL(10,2),
    IN p_notas TEXT,
    IN p_estado_id INT,
    IN p_usuario_id INT
)
BEGIN
    DECLARE EXIT HANDLER FOR SQLEXCEPTION
    BEGIN
        ROLLBACK;
        RESIGNAL;
    END;
    
    START TRANSACTION;
    
    -- Actualizar la reserva
    UPDATE reservas SET
        cliente_id = p_cliente_id,
        habitacion_id = p_habitacion_id,
        fecha_entrada = p_fecha_entrada,
        fecha_salida = p_fecha_salida,
        adultos = p_adultos,
        ninos = p_ninos,
        precio_total = p_precio_total,
        notas = p_notas,
        estado_id = p_estado_id,
        usuario_id = p_usuario_id,
        fecha_actualizacion = NOW()
    WHERE id = p_reserva_id;
    
    -- Si el estado es Check-In, actualizar estado de la habitación
    IF p_estado_id = 2 THEN -- 2 = Check-In
        UPDATE habitaciones SET estado = 'Ocupada' 
        WHERE id = p_habitacion_id;
    END IF;
    
    -- Si el estado es Check-Out, actualizar estado de la habitación
    IF p_estado_id = 3 THEN -- 3 = Check-Out
        UPDATE habitaciones SET estado = 'Limpieza' 
        WHERE id = p_habitacion_id;
    END IF;
    
    COMMIT;
END //

DELIMITER ;