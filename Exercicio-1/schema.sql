-- Banco de dados do prototipo "Reserva Senac"
-- Compatível com MySQL 8 e MySQL Workbench

CREATE DATABASE IF NOT EXISTS reserva_salas
  CHARACTER SET utf8mb4
  COLLATE utf8mb4_unicode_ci;

USE reserva_salas;

DROP TABLE IF EXISTS reserva_historico;
DROP TABLE IF EXISTS reserva;
DROP TABLE IF EXISTS sala;
DROP TABLE IF EXISTS usuario;

CREATE TABLE usuario (
  id_usuario BIGINT UNSIGNED NOT NULL AUTO_INCREMENT,
  nome VARCHAR(120) NOT NULL,
  email VARCHAR(180) NOT NULL,
  cargo VARCHAR(120) NOT NULL,
  perfil ENUM('administrador', 'gestor', 'colaborador') NOT NULL DEFAULT 'colaborador',
  ativo BOOLEAN NOT NULL DEFAULT TRUE,
  criado_em DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
  PRIMARY KEY (id_usuario),
  UNIQUE KEY uk_usuario_email (email)
) ENGINE=InnoDB;

CREATE TABLE sala (
  id_sala BIGINT UNSIGNED NOT NULL AUTO_INCREMENT,
  nome VARCHAR(120) NOT NULL,
  localizacao VARCHAR(120) NOT NULL,
  capacidade INT UNSIGNED NOT NULL,
  status ENUM('disponivel', 'indisponivel', 'manutencao') NOT NULL DEFAULT 'disponivel',
  criado_em DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
  PRIMARY KEY (id_sala),
  UNIQUE KEY uk_sala_nome (nome),
  CONSTRAINT ck_sala_capacidade CHECK (capacidade > 0)
) ENGINE=InnoDB;

CREATE TABLE reserva (
  id_reserva BIGINT UNSIGNED NOT NULL AUTO_INCREMENT,
  id_sala BIGINT UNSIGNED NOT NULL,
  id_responsavel BIGINT UNSIGNED NOT NULL,
  objetivo VARCHAR(200) NOT NULL,
  data_reserva DATE NOT NULL,
  horario_inicio TIME NOT NULL,
  horario_fim TIME NOT NULL,
  status ENUM('confirmada', 'cancelada', 'concluida') NOT NULL DEFAULT 'confirmada',
  cor VARCHAR(20) NULL,
  criado_em DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
  atualizado_em DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  PRIMARY KEY (id_reserva),
  CONSTRAINT ck_reserva_horario CHECK (horario_fim > horario_inicio),
  CONSTRAINT fk_reserva_sala
    FOREIGN KEY (id_sala) REFERENCES sala (id_sala)
    ON UPDATE CASCADE ON DELETE RESTRICT,
  CONSTRAINT fk_reserva_responsavel
    FOREIGN KEY (id_responsavel) REFERENCES usuario (id_usuario)
    ON UPDATE CASCADE ON DELETE RESTRICT,
  KEY idx_reserva_sala_data (id_sala, data_reserva, horario_inicio, horario_fim),
  KEY idx_reserva_responsavel (id_responsavel),
  KEY idx_reserva_status (status)
) ENGINE=InnoDB;

CREATE TABLE reserva_historico (
  id_historico BIGINT UNSIGNED NOT NULL AUTO_INCREMENT,
  id_reserva BIGINT UNSIGNED NOT NULL,
  id_usuario BIGINT UNSIGNED NULL,
  acao ENUM('criada', 'alterada', 'cancelada', 'concluida') NOT NULL,
  detalhes VARCHAR(500) NULL,
  registrado_em DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
  PRIMARY KEY (id_historico),
  CONSTRAINT fk_historico_reserva
    FOREIGN KEY (id_reserva) REFERENCES reserva (id_reserva)
    ON UPDATE CASCADE ON DELETE CASCADE,
  CONSTRAINT fk_historico_usuario
    FOREIGN KEY (id_usuario) REFERENCES usuario (id_usuario)
    ON UPDATE CASCADE ON DELETE SET NULL,
  KEY idx_historico_reserva (id_reserva),
  KEY idx_historico_data (registrado_em)
) ENGINE=InnoDB;

DELIMITER //

CREATE TRIGGER trg_reserva_conflito_insert
BEFORE INSERT ON reserva
FOR EACH ROW
BEGIN
  IF NEW.status = 'confirmada' AND EXISTS (
    SELECT 1
    FROM reserva r
    WHERE r.id_sala = NEW.id_sala
      AND r.data_reserva = NEW.data_reserva
      AND r.status = 'confirmada'
      AND NEW.horario_inicio < r.horario_fim
      AND NEW.horario_fim > r.horario_inicio
  ) THEN
    SIGNAL SQLSTATE '45000'
      SET MESSAGE_TEXT = 'Conflito de agenda: a sala ja esta reservada nesse periodo.';
  END IF;
END//

CREATE TRIGGER trg_reserva_conflito_update
BEFORE UPDATE ON reserva
FOR EACH ROW
BEGIN
  IF NEW.status = 'confirmada' AND EXISTS (
    SELECT 1
    FROM reserva r
    WHERE r.id_reserva <> NEW.id_reserva
      AND r.id_sala = NEW.id_sala
      AND r.data_reserva = NEW.data_reserva
      AND r.status = 'confirmada'
      AND NEW.horario_inicio < r.horario_fim
      AND NEW.horario_fim > r.horario_inicio
  ) THEN
    SIGNAL SQLSTATE '45000'
      SET MESSAGE_TEXT = 'Conflito de agenda: a sala ja esta reservada nesse periodo.';
  END IF;
END//

DELIMITER ;

-- Dados de exemplo correspondentes ao front end
INSERT INTO usuario (nome, email, cargo, perfil) VALUES
  ('Ana Silva', 'ana.silva@senac.com.br', 'Gestora de Operacoes', 'gestor'),
  ('Bruno Costa', 'bruno.costa@senac.com.br', 'Analista de Produto', 'colaborador'),
  ('Carla Mendes', 'carla.mendes@senac.com.br', 'Administradora de Escritorio', 'administrador');

INSERT INTO sala (nome, localizacao, capacidade, status) VALUES
  ('Sala Aurora', 'Torre A', 8, 'disponivel'),
  ('Sala Horizonte', 'Torre A', 12, 'disponivel'),
  ('Sala Criativa', 'Torre B', 6, 'disponivel'),
  ('Auditorio', 'Torre B', 20, 'disponivel');

INSERT INTO reserva
  (id_sala, id_responsavel, objetivo, data_reserva, horario_inicio, horario_fim, status, cor)
VALUES
  (1, 1, 'Planejamento comercial', '2026-11-17', '09:00:00', '10:30:00', 'confirmada', NULL),
  (2, 2, 'Daily de produtos', '2026-11-17', '10:00:00', '11:30:00', 'confirmada', 'orange'),
  (3, 3, 'Workshop do projeto', '2026-11-17', '13:00:00', '15:00:00', 'confirmada', 'pink'),
  (4, 1, 'Apresentacao trimestral', '2026-11-17', '14:30:00', '16:30:00', 'confirmada', 'green');

INSERT INTO reserva_historico (id_reserva, id_usuario, acao, detalhes)
SELECT id_reserva, id_responsavel, 'criada', 'Reserva importada do prototipo visual.'
FROM reserva;

CREATE OR REPLACE VIEW vw_salas_front AS
SELECT
  id_sala AS id,
  nome AS name,
  localizacao AS location,
  capacidade AS capacity,
  status
FROM sala;

CREATE OR REPLACE VIEW vw_reservas_front AS
SELECT
  r.id_reserva AS id,
  s.nome AS room,
  r.data_reserva AS date,
  r.horario_inicio AS start,
  r.horario_fim AS end,
  r.objetivo AS title,
  u.nome AS responsible,
  r.cor AS color,
  r.status
FROM reserva r
JOIN sala s ON s.id_sala = r.id_sala
JOIN usuario u ON u.id_usuario = r.id_responsavel;
