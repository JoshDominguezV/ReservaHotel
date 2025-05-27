-- SP para obtener todas las habitaciones con información de tipo
DELIMITER //
CREATE PROCEDURE sp_GetAllRooms()
BEGIN
    SELECT 
        h.id,
        h.numero,
        th.nombre AS tipo,
        th.descripcion,
        th.capacidad,
        th.precio_base AS precio,
        h.piso,
        h.estado,
        h.ultima_limpieza,
        h.notas
    FROM 
        habitaciones h
    JOIN 
        tipos_habitacion th ON h.tipo_id = th.id;
END //
DELIMITER ;

-- SP para buscar habitaciones
DELIMITER //
CREATE PROCEDURE sp_SearchRooms(IN p_search VARCHAR(100))
BEGIN
    SELECT 
        h.id,
        h.numero,
        th.nombre AS tipo,
        th.capacidad,
        th.precio_base AS precio,
        h.piso,
        h.estado
    FROM 
        habitaciones h
    JOIN 
        tipos_habitacion th ON h.tipo_id = th.id
    WHERE 
        h.numero LIKE CONCAT('%', p_search, '%') OR
        th.nombre LIKE CONCAT('%', p_search, '%') OR
        h.estado LIKE CONCAT('%', p_search, '%');
END //
DELIMITER ;

-- SP para crear una nueva habitación
DELIMITER //
CREATE PROCEDURE sp_CreateRoom(
    IN p_numero VARCHAR(10),
    IN p_tipo_id INT,
    IN p_piso INT,
    IN p_estado VARCHAR(20),
    IN p_notas TEXT
)
BEGIN
    INSERT INTO habitaciones (numero, tipo_id, piso, estado, notas)
    VALUES (p_numero, p_tipo_id, p_piso, p_estado, p_notas);
    
    SELECT LAST_INSERT_ID() AS new_id;
END //
DELIMITER ;

-- SP para actualizar una habitación
DELIMITER //
CREATE PROCEDURE sp_UpdateRoom(
    IN p_id INT,
    IN p_numero VARCHAR(10),
    IN p_tipo_id INT,
    IN p_piso INT,
    IN p_estado VARCHAR(20),
    IN p_notas TEXT
)
BEGIN
    UPDATE habitaciones
    SET 
        numero = p_numero,
        tipo_id = p_tipo_id,
        piso = p_piso,
        estado = p_estado,
        notas = p_notas
    WHERE id = p_id;
END //
DELIMITER ;

-- SP para eliminar una habitación
DELIMITER //
CREATE PROCEDURE sp_DeleteRoom(IN p_id INT)
BEGIN
    DELETE FROM habitaciones WHERE id = p_id;
END //
DELIMITER ;

-- SP para obtener tipos de habitación
DELIMITER //
CREATE PROCEDURE sp_GetRoomTypes()
BEGIN
    SELECT id, nombre, descripcion, capacidad, precio_base FROM tipos_habitacion;
END //
DELIMITER ;