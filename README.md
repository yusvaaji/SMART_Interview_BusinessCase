# construction-smart-test
construction-smart-test

**Part 1 : Backend** (.net core , Kafka , ElasticSearch)

- Make sure all nuget package installed (**dotnet restore** command)

- **appsettings.json**
    "ConnectionStrings": {
        "DefaultConnection": "Host=localhost;Port=5432;Database=postgres;Username=postgres;Password=grafik123" // put your database connection string
      },
  
      "Kafka": {
        "BootstrapServers": "kafka-9644aa4-grafikyusvaaji-d9b0.k.aivencloud.com:19541",
        "Topic": "es.construction.hbx",
        "SaslMechanism": "PLAIN",
        "SecurityProtocol": "SASL_SSL", 
        "SaslUsername": "your_kafka_username",
        "SaslPassword": "your_kafka_password",
        "SslCaLocation": "your_project_path\\cert\\to\\ca-cert\\ca.pem"  // you can change with your own kafka settings
      },
  
      "ElasticSearch": {
        "Uri": "http://localhost:9200", // make sure elasticsearch services running on this Uri
        "DefaultIndex": "constructionprojects" // default index that used on elasticsearch services
      },
  
      "Jwt": {
        "Key": "your_new_secret_key_which_is_at_least_16_chars",
        "Issuer": "grafikIssuer",
        "Audience": "grafikAudience"
      }
- run "**dotnet build** command" , once Backend services **running**, you can access **https://localhost:7051/index.html** for API Documentation

**Part 2 : Frontend** ( Vue js ) 
- Make sure all package installed (npm install / yarn )
- Make sure 
  baseURL: 'https://localhost:7051/api' (**services/axios.js**) is correct for comminication within **Backend**

**Part 3 : Database (using postgres)**

CREATE TABLE public."ConstructionProjects" (
	"ProjectId" varchar(6) NOT NULL,
	"ProjectName" varchar(200) NOT NULL,
	"ProjectLocation" varchar(500) NOT NULL,
	"ProjectStage" int4 NOT NULL,
	"ProjectCategory" int4 NOT NULL,
	"OtherCategory" text NOT NULL,
	"ProjectConstructionStartDate" timestamptz NOT NULL,
	"ProjectDetails" varchar(2000) NOT NULL,
	"ProjectCreatorId" uuid NOT NULL,
	CONSTRAINT "PK_ConstructionProjects" PRIMARY KEY ("ProjectId")
);
CREATE UNIQUE INDEX "IX_ConstructionProjects_ProjectName" ON public."ConstructionProjects" USING btree ("ProjectName");

CREATE TABLE public."Users" (
	"UserId" uuid NOT NULL,
	"Email" text NOT NULL,
	"Password" text NOT NULL,
	CONSTRAINT "PK_Users" PRIMARY KEY ("UserId")
);


**store procedure section**
    
-- Create stored procedures for CRUD operations on the ConstructionProjects table

-- Procedure to create a new project
CREATE OR REPLACE FUNCTION public.create_project(
    p_projectid varchar,
    p_projectname varchar,
    p_projectlocation varchar,
    p_projectstage int,
    p_projectcategory int,
    p_othercategory text,
    p_projectconstructionstartdate timestamptz,
    p_projectdetails varchar,
    p_projectcreatorid uuid
)
RETURNS void AS $$
BEGIN
    INSERT INTO public."ConstructionProjects" (
        "ProjectId", "ProjectName", "ProjectLocation", "ProjectStage", "ProjectCategory", "OtherCategory", "ProjectConstructionStartDate", "ProjectDetails", "ProjectCreatorId"
    ) VALUES (
        p_projectid, p_projectname, p_projectlocation, p_projectstage, p_projectcategory, p_othercategory, p_projectconstructionstartdate, p_projectdetails, p_projectcreatorid
    );
END;
$$ LANGUAGE plpgsql;

-- Procedure to read a project by ProjectId
CREATE OR REPLACE FUNCTION public.read_project(
    p_projectid varchar
)
RETURNS TABLE (
    "ProjectId" varchar,
    "ProjectName" varchar,
    "ProjectLocation" varchar,
    "ProjectStage" int,
    "ProjectCategory" int,
    "OtherCategory" text,
    "ProjectConstructionStartDate" timestamptz,
    "ProjectDetails" varchar,
    "ProjectCreatorId" uuid
) AS $$
BEGIN
    RETURN QUERY
    SELECT
        "ProjectId", "ProjectName", "ProjectLocation", "ProjectStage", "ProjectCategory", "OtherCategory", "ProjectConstructionStartDate", "ProjectDetails", "ProjectCreatorId"
    FROM public."ConstructionProjects"
    WHERE "ProjectId" = p_projectid;
END;
$$ LANGUAGE plpgsql;

-- Procedure to update a project by ProjectId
CREATE OR REPLACE FUNCTION public.update_project(
    p_projectid varchar,
    p_projectname varchar,
    p_projectlocation varchar,
    p_projectstage int,
    p_projectcategory int,
    p_othercategory text,
    p_projectconstructionstartdate timestamptz,
    p_projectdetails varchar,
    p_projectcreatorid uuid
)
RETURNS void AS $$
BEGIN
    UPDATE public."ConstructionProjects"
    SET
        "ProjectName" = p_projectname,
        "ProjectLocation" = p_projectlocation,
        "ProjectStage" = p_projectstage,
        "ProjectCategory" = p_projectcategory,
        "OtherCategory" = p_othercategory,
        "ProjectConstructionStartDate" = p_projectconstructionstartdate,
        "ProjectDetails" = p_projectdetails,
        "ProjectCreatorId" = p_projectcreatorid
    WHERE "ProjectId" = p_projectid;
END;
$$ LANGUAGE plpgsql;

-- Procedure to delete a project by ProjectId
CREATE OR REPLACE FUNCTION public.delete_project(
    p_projectid varchar
)
RETURNS void AS $$
BEGIN
    DELETE FROM public."ConstructionProjects"
    WHERE "ProjectId" = p_projectid;
END;
$$ LANGUAGE plpgsql;
   
