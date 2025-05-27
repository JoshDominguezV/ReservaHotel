--  procedimientos almacenados ped_hotel_reserva.sql

-- Procedimiento para obtener todos los usuarios con sus roles
DELIMITER //
CREATE PROCEDURE sp_GetAllUsersWithRoles()
BEGIN
    SELECT u.id, u.nombre_usuario, r.nombre_rol 
    FROM usuarios u
    JOIN roles r ON u.rol_id = r.id;
END //
DELIMITER ;

-- Procedimiento para crear un nuevo usuario
DELIMITER //
CREATE PROCEDURE sp_CreateUser(
    IN p_nombre_usuario VARCHAR(50),
    IN p_contrasena VARCHAR(255),
    IN p_rol_id INT
)
BEGIN
    INSERT INTO usuarios (nombre_usuario, contrasena, rol_id)
    VALUES (p_nombre_usuario, p_contrasena, p_rol_id);
END //
DELIMITER ;

-- Procedimiento para actualizar un usuario
DELIMITER //
CREATE PROCEDURE sp_UpdateUser(
    IN p_id INT,
    IN p_nombre_usuario VARCHAR(50),
    IN p_rol_id INT
)
BEGIN
    UPDATE usuarios 
    SET nombre_usuario = p_nombre_usuario, 
        rol_id = p_rol_id
    WHERE id = p_id;
END //
DELIMITER ;

-- Procedimiento para eliminar un usuario
DELIMITER //
CREATE PROCEDURE sp_DeleteUser(IN p_id INT)
BEGIN
    DELETE FROM usuarios WHERE id = p_id;
END //
DELIMITER ;

-- Procedimiento para obtener todos los roles
DELIMITER //
CREATE PROCEDURE sp_GetAllRoles()
BEGIN
    SELECT id, nombre_rol FROM roles;
END //
DELIMITER ;