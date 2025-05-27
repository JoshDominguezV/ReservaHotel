DELIMITER //

-- SP to create a new client
CREATE PROCEDURE sp_CreateClient(
    IN p_nombre VARCHAR(100),
    IN p_apellido VARCHAR(100),
    IN p_tipo_documento ENUM('DNI', 'Pasaporte', 'Carnet Extranjería', 'Otro'),
    IN p_documento_identidad VARCHAR(20),
    IN p_telefono VARCHAR(20),
    IN p_correo VARCHAR(100)
)
BEGIN
    INSERT INTO clientes (nombre, apellido, tipo_documento, documento_identidad, 
                         telefono, correo)
    VALUES (p_nombre, p_apellido, p_tipo_documento, p_documento_identidad,
            p_telefono, p_correo);
    
    SELECT LAST_INSERT_ID() AS nuevo_id;
END //

-- SP to update a client
CREATE PROCEDURE sp_UpdateClient(
    IN p_id INT,
    IN p_nombre VARCHAR(100),
    IN p_apellido VARCHAR(100),
    IN p_tipo_documento ENUM('DNI', 'Pasaporte', 'Carnet Extranjería', 'Otro'),
    IN p_documento_identidad VARCHAR(20),
    IN p_telefono VARCHAR(20),
    IN p_correo VARCHAR(100)
)
BEGIN
    UPDATE clientes 
    SET nombre = p_nombre,
        apellido = p_apellido,
        tipo_documento = p_tipo_documento,
        documento_identidad = p_documento_identidad,
        telefono = p_telefono,
        correo = p_correo,
        fecha_registro = fecha_registro -- Keep original date
    WHERE id = p_id;
END //

-- SP to get all clients
CREATE PROCEDURE sp_GetClients()
BEGIN
    SELECT id, CONCAT(nombre, ' ', IFNULL(apellido, '')) AS nombre_completo, 
           tipo_documento, documento_identidad, telefono, correo
    FROM clientes
    ORDER BY nombre, apellido;
END //

-- SP to search clients by name or document
CREATE PROCEDURE sp_SearchClients(IN p_busqueda VARCHAR(100))
BEGIN
    SELECT id, CONCAT(nombre, ' ', IFNULL(apellido, '')) AS nombre_completo, 
           tipo_documento, documento_identidad, telefono, correo
    FROM clientes
    WHERE nombre LIKE CONCAT('%', p_busqueda, '%') OR 
          apellido LIKE CONCAT('%', p_busqueda, '%') OR
          documento_identidad LIKE CONCAT('%', p_busqueda, '%')
    ORDER BY nombre, apellido;
END //

-- SP to get details of a specific client
CREATE PROCEDURE sp_GetClientById(IN p_id INT)
BEGIN
    SELECT * FROM clientes WHERE id = p_id;
END //

-- SP to delete a client
CREATE PROCEDURE sp_DeleteClient(IN p_id INT)
BEGIN
    DELETE FROM clientes WHERE id = p_id;
END //

DELIMITER ;
