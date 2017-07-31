 DROP SCHEMA IF EXISTS db CASCADE;
 CREATE SCHEMA db;
 SET SEARCH_PATH TO db, public;
 
 CREATE TABLE "article" (
	"id" serial NOT NULL,
	"fkGroup" integer NOT NULL,
	"sourceUrl" TEXT NOT NULL,
	"articleIsTrue" BOOLEAN NOT NULL,
	"fkScreenshot" integer NOT NULL,
	"heading" TEXT NOT NULL,
	"summary" TEXT NOT NULL,
	"metricsAlteration" integer,
	"metricsTrue" BOOLEAN, 
	"fbShares" integer,
	"fbLikes" integer,
	"fbComments" integer,
	"twRetweets" integer,
	"twLikes" integer,
	
	CONSTRAINT "metricsAlteration_valid" CHECK ("metricsAlteration" BETWEEN -1 and +1),
	CONSTRAINT article_pk PRIMARY KEY ("id")
) WITH (
  OIDS=FALSE
);

-- #Maybe?
-- ALTER TABLE article ADD CONSTRAINT "metricsAlteration_articleIsTrue" CHECK ("articleIsTrue" AND "metricsAlteration" = 0);

CREATE TABLE "image" (
	"id" serial NOT NULL,
	"data" bytea NOT NULL,
	"contentType" TEXT NOT NULL,
	CONSTRAINT image_pk PRIMARY KEY ("id")
) WITH (
  OIDS=FALSE
);

CREATE TABLE "vote" (
	"id" serial NOT NULL,
	"value" BOOLEAN NOT NULL,
	"fkArticle" integer NOT NULL,
	"ts" TIMESTAMP DEFAULT now() NOT NULL,
	CONSTRAINT vote_pk PRIMARY KEY ("id")
) WITH (
  OIDS=FALSE
);

CREATE TABLE "articleGroup" (
	"id" serial NOT NULL,
	CONSTRAINT articleGroup_pk PRIMARY KEY ("id")
) WITH (
  OIDS=FALSE
);

ALTER TABLE "article" ADD CONSTRAINT "article_fkGropu"      FOREIGN KEY ("fkGroup")      REFERENCES "articleGroup"("id");
ALTER TABLE "article" ADD CONSTRAINT "article_fkScreenshot" FOREIGN KEY ("fkScreenshot") REFERENCES "image"("id");
ALTER TABLE "vote"    ADD CONSTRAINT "vote_fkArticle"       FOREIGN KEY ("fkArticle")    REFERENCES "article"("id");

DROP FUNCTION IF EXISTS getNextArticleId();
DROP FUNCTION IF EXISTS getNextArticle();
DROP FUNCTION IF EXISTS getStatisticsForVote(INTEGER);
DROP FUNCTION IF EXISTS setVote(INTEGER,BOOLEAN);

CREATE OR REPLACE FUNCTION getNextArticleId()
  RETURNS TABLE(id INTEGER) as $$
BEGIN
  RETURN QUERY SELECT a.id
  FROM article a
  WHERE a."fkGroup" NOT IN (
    -- exclude 25% of the last used article groups
    SELECT "fkGroup"
      FROM vote v1
      LEFT JOIN article a1 ON v1."fkArticle" = a1.id
      LEFT JOIN "articleGroup" g1 ON a1."fkGroup" = g1.id
    ORDER BY v1.id DESC
    LIMIT (SELECT count(distinct "fkGroup") * 0.25 FROM article)
  )
  ORDER BY
    -- votes count by articleGroup
    (SELECT count(*) FROM vote v LEFT JOIN article va on v."fkArticle" = va.id WHERE va."fkGroup" = a."fkGroup") ASC,
    random()
  LIMIT 1;
END;
$$ LANGUAGE plpgsql;

CREATE OR REPLACE FUNCTION getNextArticle() RETURNS SETOF article as $$
BEGIN
  RETURN QUERY
  SELECT *
  FROM article
  WHERE id IN (SELECT getnextarticleid())
  LIMIT 1;
END;
$$ LANGUAGE plpgsql;

CREATE OR REPLACE FUNCTION setVote(article_id INTEGER, vote_value BOOLEAN)
RETURNS INTEGER AS $$
DECLARE
  rval INTEGER;
BEGIN
  INSERT INTO vote (value, "fkArticle") VALUES (vote_value, article_id) RETURNING id INTO rval;
  RETURN rval;
END;
$$ LANGUAGE plpgsql;

CREATE OR REPLACE FUNCTION getStatisticsForVote("voteId" INTEGER)
  RETURNS TABLE (
    vote BOOLEAN,
    "articleIsTrue" BOOLEAN,
    "metricsTrue" BOOLEAN,
    "votedCorrect" BOOLEAN, -- correct guess/answer/vote
    "sameVote" BIGINT, -- n=sameVote/(allVotes/100) => n% voted like you
    "allVotes" BIGINT
  ) as $$
BEGIN

  RETURN QUERY SELECT
    v.value as vote,
    a."articleIsTrue",
    a."metricsTrue",
    (v.value = a."articleIsTrue") as "votedCorrect",
    (SELECT count(*) FROM vote vs WHERE vs."fkArticle" = v."fkArticle" AND vs.value = v.value ) as "sameVote",
    (SELECT count(*) FROM vote va WHERE va."fkArticle" = v."fkArticle") as "allVotes"
  FROM vote v
  LEFT JOIN article a on v."fkArticle" = a.id
  WHERE v.id = "voteId";

END;
$$ LANGUAGE plpgsql;
