DELIMITER //

-- SP para obtener todos los check-ins (sin filtrar por fecha)
CREATE PROCEDURE sp_GetAllCheckInsForCheckOut()
BEGIN
    SELECT 
        ci.id AS checkin_id,
        r.id AS reserva_id,
        CONCAT(c.nombre, ' ', c.apellido) AS cliente,
        h.numero AS habitacion,
        th.nombre AS tipo_habitacion,
        r.fecha_entrada,
        r.fecha_salida,
        DATEDIFF(r.fecha_salida, r.fecha_entrada) AS noches,
        r.precio_total,
        ci.metodo_pago,
        ci.deposito_seguridad,
        ci.fecha_hora AS fecha_checkin,
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
        r.estado_id = 2 -- Solo check-ins (estado Check-In)
    ORDER BY 
        r.fecha_salida;
END //

-- SP para realizar el check-out
CREATE PROCEDURE sp_PerformCheckOut(
    IN p_checkin_id INT,
    IN p_reserva_id INT,
    IN p_usuario_id INT,
    IN p_estado_habitacion ENUM('Excelente', 'Bueno', 'Daños menores', 'Daños graves'),
    IN p_cobros_adicionales DECIMAL(10,2),
    IN p_devolucion_deposito DECIMAL(10,2),
    IN p_observaciones TEXT
)
BEGIN
    DECLARE v_habitacion_id INT;
    
    -- Obtener ID de la habitación
    SELECT habitacion_id INTO v_habitacion_id FROM reservas WHERE id = p_reserva_id;
    
    -- Registrar el check-out
    INSERT INTO checkouts (
        reserva_id,
        usuario_id,
        estado_habitacion,
        cobros_adicionales,
        devolucion_deposito,
        observaciones
    ) VALUES (
        p_reserva_id,
        p_usuario_id,
        p_estado_habitacion,
        p_cobros_adicionales,
        p_devolucion_deposito,
        p_observaciones
    );
    
    -- Actualizar estado de la reserva a Check-Out (3)
    UPDATE reservas SET estado_id = 3 WHERE id = p_reserva_id;
    
    -- Actualizar estado de la habitación a Limpieza
    UPDATE habitaciones SET estado = 'Limpieza' WHERE id = v_habitacion_id;
    
    SELECT LAST_INSERT_ID() AS checkout_id;
END //

DELIMITER ;