-- Crear la tabla Tarjeta (si no existe)
IF OBJECT_ID('dbo.Tarjeta', 'U') IS NOT NULL 
   DROP TABLE dbo.Tarjeta;

CREATE TABLE dbo.Tarjeta (
    Id INT PRIMARY KEY,
    NombreTitular NVARCHAR(100),
    NumeroTarjeta CHAR(16),
    Estado NVARCHAR(50),
    Activa BIT
);

-- Variables para generación aleatoria
DECLARE @i INT = 1;
DECLARE @max INT = 500000;

WHILE @i <= @max
BEGIN
    INSERT INTO dbo.Tarjeta (Id, NombreTitular, NumeroTarjeta, Estado, Activa)
    VALUES (
        @i,
        -- Generación sencilla de nombre: concatenación de 'Titular' + número
        CONCAT('Titular ', @i),
        -- Número de tarjeta: 16 dígitos a partir de @i con ceros a la izquierda
        RIGHT('0000000000000000' + CAST(@i AS VARCHAR(16)), 16),
        -- Estado: rotar en varios estados posibles usando módulo
        CASE (@i % 5)
            WHEN 0 THEN 'Nuevo'
            WHEN 1 THEN 'Activo'
            WHEN 2 THEN 'Bloqueado'
            WHEN 3 THEN 'Cancelado'
            ELSE 'Pendiente'
        END,
        -- Activa: 1 si @i es impar, 0 si par
        CASE WHEN (@i % 2) = 1 THEN 1 ELSE 0 END
    );

    SET @i = @i + 1;
END;
