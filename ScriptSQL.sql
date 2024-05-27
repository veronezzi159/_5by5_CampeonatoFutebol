CREATE DATABASE CampeonatoFutebol;

USE CampeonatoFutebol;

CREATE TABLE Equipe(
	Id int IDENTITY(1,1),
	Nome varchar(50) not null,
	Apelido varchar(30),
	DataCriacao DATE not null,
	Pontuacao int,
	TotalGolsMarcados int,
	TotalGolsSofridos int,
	CONSTRAINT pk_time primary key (Id),
	CONSTRAINT un_nome unique (Nome)

);

CREATE TABLE Jogo (
	TimeDaCasa int,
	TimeVisitante int,
	Codigo int IDENTITY(1,1),
	GolsDaCasa int,
	GolsVisi int,
	TotalGols int,
	CONSTRAINT pk_jogo PRIMARY KEY(TimeDaCasa,TimeVisitante),
	CONSTRAINT fk_time_casa FOREIGN KEY (TimeDaCasa) REFERENCES Equipe (Id),
	CONSTRAINT fk_time_visitante FOREIGN KEY (TimeVisitante) REFERENCES Equipe (Id),
	CONSTRAINT un_codigo unique (Codigo)

);

CREATE TABLE JogoMaisGolsTime(
	IdTime int,
	Jogo int not null,
	Gols int,
	CONSTRAINT pk_jogo_mais_gols_time primary key (IdTime),
	CONSTRAINT fk_idTime FOREIGN KEY (IdTime) REFERENCES Equipe (Id),
	CONSTRAINT fk_jogo FOREIGN KEY (Jogo) REFERENCES Jogo (Codigo)
	
);
--FUNÇÃO PARA INSERIR Equipe
CREATE PROCEDURE InserirEquipe @Nome varchar(50), @apelido varchar(30), @dataCriacao date
As
INSERT INTO Equipe (Nome,Apelido,DataCriacao)
VALUES (@Nome,@apelido,@dataCriacao)
;


--Inserir jogo
CREATE PROCEDURE InserirJogo  @idTimeCasa int, @idTimeVisitante int,
@golsCasa int, @golsVisitante int
AS
	INSERT INTO Jogo (TimeDaCasa,TimeVisitante,GolsDaCasa,GolsVisi)
	VALUES (@idTimeCasa,@idTimeVisitante,@golsCasa,@golsVisitante);

	
	UPDATE Jogo
	SET TotalGols = @golsCasa + @golsVisitante
	WHERE TimeDaCasa = @idTimeCasa AND TimeVisitante = @idTimeVisitante;
	
;

--EXEC InserirJogo 1,2,3,4;
--EXEC InserirJogo 2,1,2,4;




-- Retorna time com mais gols

CREATE OR ALTER PROCEDURE RetornarTimeComMaisGols
AS 
	SELECT TOP(1) Nome,TotalGolsMarcados FROM Equipe
	ORDER BY TotalGolsMarcados DESC, TotalGolsSofridos ASC


	RETURN;
;

-- retornar time com mais gols sofridos

CREATE OR ALTER PROCEDURE RetornarTimeComMaisGolsSofridos
AS
	SELECT TOP(1) Nome,TotalGolsSofridos FROM Equipe
	ORDER BY TotalGolsSofridos DESC, TotalGolsMarcados ASC


	RETURN;
;

-- Jogo que teve mais gols

CREATE OR ALTER PROCEDURE RetornarJogoComMaisGols
AS 
	SELECT TOP(1) e.Nome AS TimeCasa,ev.Nome AS TimeVisitante,j.GolsDaCasa,j.GolsVisi ,j.TotalGols FROM Jogo AS j
	INNER JOIN Equipe AS e ON (j.TimeDaCasa = e.Id)	
	INNER JOIN Equipe AS ev ON (j.TimeVisitante = ev.Id)	
	ORDER BY j.TotalGols DESC

	RETURN;
;

-- Verifica equipe existe na tabela jogoMaisGolsTime

CREATE OR ALTER  PROCEDURE CriaTimeEmJogoMaisGolsTime @time int, @cod int, @gols int
AS
	IF NOT EXISTS(SELECT IdTime FROM JogoMaisGolsTime where IdTime = @time)
	BEGIN	
		INSERT INTO JogoMaisGolsTime 
		VALUES(@time,@cod,@gols )
	END
;
--EXEC RetornarJogoMaisGolsTime

-- Retorna maior numero de gols de cada time fez em um único jogo
CREATE OR ALTER PROCEDURE RetornarJogoMaisGolsTime 
AS
	SELECT EQ.Nome,JG.Gols  FROM JogoMaisGolsTime AS JG
	INNER JOIN Equipe AS EQ ON (JG.IdTime = EQ.Id)

	RETURN;
;
-- Retorna campeão
CREATE OR ALTER PROCEDURE RetornarCampeao
AS
	SELECT TOP(1) Nome, Pontuacao, TotalGolsMarcados, TotalGolsSofridos FROM Equipe
	ORDER BY Pontuacao DESC, TotalGolsMarcados Desc, TotalGolsSofridos ASC

	RETURN;

;

-- retorna o Ranking

CREATE OR ALTER PROC RetornarRanking
AS
	SELECT  Nome, Pontuacao, TotalGolsMarcados, TotalGolsSofridos FROM Equipe
	ORDER BY Pontuacao DESC, TotalGolsMarcados Desc, TotalGolsSofridos ASC

	RETURN;
;

EXEC RetornarRanking
-- reiniciar campeonato com mesmo times

CREATE OR ALTER PROC ReiniciarCampeonato
AS
	delete JogoMaisGolsTime;
	delete Jogo;

	 UPDATE Equipe
	 SET Pontuacao = 0, TotalGolsMarcados = 0, TotalGolsSofridos = 0;
	
;

--Reseta todas as tabelas para começar outro campeonato

CREATE OR ALTER PROCEDURE ResetarTabelas
AS

DELETE JogoMaisGolsTime
DELETE Jogo
DELETE Equipe

DBCC CHECKIDENT(Jogo, RESEED, 0)

DBCC CHECKIDENT(Equipe, RESEED, 0)

;
-- TRIGGER ZERAS PONTOS/GOLS

CREATE OR ALTER TRIGGER [TG_ZerarPontos]
ON [Equipe] 
AFTER INSERT
AS 
DECLARE @id int;
BEGIN


	SELECT @id = Id
	FROM INSERTED;
	

	UPDATE Equipe
	SET Pontuacao = 0, TotalGolsMarcados = 0, TotalGolsSofridos = 0
	WHERE Id = @id;

END;


CREATE OR ALTER TRIGGER [TG_InserirPontos]
ON [Jogo] 
AFTER INSERT
AS 
DECLARE @codigo int,
@golsCasa int,
@golsVisi int,
@idTimeCasa int,
@idTimeVisi int;
BEGIN

	SELECT @codigo = Codigo, @golsCasa =GolsDaCasa, @golsVisi = GolsVisi, @idTimeCasa = TimeDaCasa, @idTimeVisi = TimeVisitante
	FROM INSERTED;
	
	IF (@golsCasa > @golsVisi)
	BEGIN

		--insere pontos e gols
		UPDATE Equipe 
		SET Pontuacao = Pontuacao + 3, TotalGolsMarcados = TotalGolsMarcados + @golsCasa, TotalGolsSofridos = TotalGolsSofridos + @golsVisi
		WHERE Id = @idTimeCasa

		--insere gols
		UPDATE Equipe 
		SET  TotalGolsMarcados = TotalGolsMarcados + @golsVisi, TotalGolsSofridos += @golsCasa
		WHERE Id = @idTimeVisi
	END
	 ELSE 
		IF (@golsCasa < @golsVisi)
		BEGIN 

			--insere pontos e gols
			UPDATE Equipe 
			SET Pontuacao += 5, TotalGolsMarcados += @golsVisi, TotalGolsSofridos += @golsCasa
			WHERE Id = @idTimeVisi

			--insere gols
			UPDATE Equipe 
			SET TotalGolsMarcados += @golsCasa, TotalGolsSofridos += @golsVisi
			WHERE Id = @idTimeCasa
		END
		ELSE
		BEGIN
			--insere pontos e gols
			UPDATE Equipe 
			SET Pontuacao += 1, TotalGolsMarcados += @golsVisi, TotalGolsSofridos += @golsCasa
			WHERE Id = @idTimeVisi

			--insere pontos e gols
			UPDATE Equipe 
			SET Pontuacao += 1, TotalGolsMarcados += @golsCasa, TotalGolsSofridos += @golsVisi
			WHERE Id = @idTimeCasa
		END
END;

CREATE or alter TRIGGER TG_InserirJogoMaisGolsTime
ON JOGO
AFTER INSERT 
AS
DECLARE @golsCasa int, @golsVisi int,@idTimeCasa int, @idTimeVisi int, 
@codigo int, @golsAtuais int
BEGIN
-- PEGA OS VALORES DO JOGO INSERIDO
	SELECT @idTimeCasa = TimeDaCasa, @idTimeVisi = TimeVisitante,
	@codigo = Codigo, @golsCasa = GolsDaCasa, @golsVisi = GolsVisi
	FROM inserted;

--VERIFCA SE O TIME EXISTE NA TABELA, SE NÃO INSERE
	EXEC CriaTimeEmJogoMaisGolsTime @idTimeCasa, @codigo, @golsCasa;

	EXEC CriaTimeEmJogoMaisGolsTime @idTimeVisi, @codigo, @golsVisi;
		

	SELECT @golsAtuais = Gols FROM JogoMaisGolsTime
		WHERE IdTime = @idTimeCasa
	
	IF (@golsAtuais < @golsCasa)
		BEGIN
			UPDATE JogoMaisGolsTime
			SET Jogo = @codigo, Gols = @golsCasa
			WHERE IdTime =  @idTimeCasa;	
		END
	SET @golsAtuais = 0;

	SELECT @golsAtuais = Gols FROM JogoMaisGolsTime
		WHERE IdTime = @idTimeVisi

	IF(@golsAtuais < @golsVisi)
		BEGIN
			UPDATE JogoMaisGolsTime
			SET Jogo = @codigo, Gols = @golsVisi
			WHERE IdTime =  @idTimeVisi;	
		END
	
END;

