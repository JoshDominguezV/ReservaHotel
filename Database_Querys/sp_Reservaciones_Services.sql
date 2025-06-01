DELIMITER //

-- SP para crear una nueva reserva
CREATE PROCEDURE sp_CreateReservation(
    IN p_cliente_id INT,
    IN p_habitacion_id INT,
    IN p_usuario_id INT,
    IN p_fecha_entrada DATE,
    IN p_fecha_salida DATE,
    IN p_adultos INT,
    IN p_ninos INT,
    IN p_precio_total DECIMAL(10,2),
    IN p_notas TEXT
)
BEGIN
    -- Estado 1 = Pendiente
    INSERT INTO reservas (cliente_id, habitacion_id, usuario_id, estado_id, 
                          fecha_entrada, fecha_salida, adultos, ninos, precio_total, notas)
    VALUES (p_cliente_id, p_habitacion_id, p_usuario_id, 1,
            p_fecha_entrada, p_fecha_salida, p_adultos, p_ninos, p_precio_total, p_notas);
    
    -- Actualizar estado de la habitación a "Ocupada"
    UPDATE habitaciones SET estado = 'Ocupada' WHERE id = p_habitacion_id;
    
    SELECT LAST_INSERT_ID() AS nueva_reserva_id;
END //

-- SP para obtener reservas activas
CREATE PROCEDURE sp_GetActiveReservations()
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
        er.nombre AS estado
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
        r.estado_id IN (1, 2) -- Pendiente o Check-In
    ORDER BY 
        r.fecha_entrada;
END //

-- SP para buscar clientes para reservación
CREATE PROCEDURE sp_SearchClientsForReservation(IN p_search VARCHAR(100))
BEGIN
    SELECT 
        id,
        CONCAT(nombre, ' ', IFNULL(apellido, '')) AS nombre_completo,
        tipo_documento,
        documento_identidad,
        telefono,
        correo
    FROM 
        clientes
    WHERE 
        nombre LIKE CONCAT('%', p_search, '%') OR 
        apellido LIKE CONCAT('%', p_search, '%') OR
        documento_identidad LIKE CONCAT('%', p_search, '%')
    ORDER BY 
        nombre, apellido
    LIMIT 20;
END //

-- SP para obtener habitaciones disponibles en un rango de fechas
CREATE PROCEDURE sp_GetAvailableRooms(
    IN p_fecha_entrada DATE,
    IN p_fecha_salida DATE,
    IN p_tipo_habitacion_id INT
)
BEGIN
    SELECT 
        h.id,
        h.numero,
        th.nombre AS tipo,
        th.descripcion,
        th.capacidad,
        th.precio_base AS precio,
        h.piso,
        h.estado
    FROM 
        habitaciones h
    JOIN 
        tipos_habitacion th ON h.tipo_id = th.id
    WHERE 
        h.estado = 'Disponible'
        AND (p_tipo_habitacion_id = 0 OR h.tipo_id = p_tipo_habitacion_id)
        AND h.id NOT IN (
            SELECT habitacion_id 
            FROM reservas 
            WHERE estado_id IN (1, 2) -- Pendiente o Check-In
            AND (
                (fecha_entrada BETWEEN p_fecha_entrada AND p_fecha_salida) OR
                (fecha_salida BETWEEN p_fecha_entrada AND p_fecha_salida) OR
                (p_fecha_entrada BETWEEN fecha_entrada AND fecha_salida) OR
                (p_fecha_salida BETWEEN fecha_entrada AND fecha_salida)
            )
        );
END //

-- SP para obtener tipos de habitación
CREATE PROCEDURE sp_GetRoomTypesForReservation()
BEGIN
    SELECT id, nombre, descripcion, capacidad, precio_base 
    FROM tipos_habitacion
    ORDER BY nombre;
END //

-- SP para actualizar estado de reserva
CREATE PROCEDURE sp_UpdateReservationStatus(
    IN p_reserva_id INT,
    IN p_estado_id INT
)
BEGIN
    UPDATE reservas SET estado_id = p_estado_id WHERE id = p_reserva_id;
    
    -- Si es check-out, liberar la habitación
    IF p_estado_id = 3 THEN -- 3 = Check-Out
        UPDATE habitaciones h
        JOIN reservas r ON h.id = r.habitacion_id
        SET h.estado = 'Disponible'
        WHERE r.id = p_reserva_id;
    END IF;
END //

-- SP para agregar servicio a una reserva
CREATE PROCEDURE sp_AddServiceToReservation(
    IN p_reserva_id INT,
    IN p_servicio_id INT,
    IN p_cantidad INT,
    IN p_fecha DATE,
    IN p_precio_unitario DECIMAL(10,2),
    IN p_notas TEXT
)
BEGIN
    INSERT INTO servicios_reserva (reserva_id, servicio_id, cantidad, fecha, precio_unitario, notas)
    VALUES (p_reserva_id, p_servicio_id, p_cantidad, p_fecha, p_precio_unitario, p_notas);
    
    SELECT LAST_INSERT_ID() AS nuevo_id;
END //

-- SP para obtener servicios de una reserva
CREATE PROCEDURE sp_GetReservationServices(IN p_reserva_id INT)
BEGIN
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

-- SP para eliminar servicio de reserva
CREATE PROCEDURE sp_RemoveServiceFromReservation(IN p_id INT)
BEGIN
    DELETE FROM servicios_reserva WHERE id = p_id;
END //

-- SP para obtener servicios adicionales disponibles
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
        activo = TRUE
    ORDER BY 
        nombre;
END //


DELIMITER ;
