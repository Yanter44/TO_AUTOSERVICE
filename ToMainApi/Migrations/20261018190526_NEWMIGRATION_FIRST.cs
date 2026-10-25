using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace ToMainApi.Migrations
{
    /// <inheritdoc />
    public partial class NEWMIGRATION_FIRST : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ApplicationProvidedDocuments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    DocumentType = table.Column<string>(type: "text", nullable: false),
                    DisplayName = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApplicationProvidedDocuments", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PhotoGallery",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Tag = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Group = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    PublicId = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    Url = table.Column<string>(type: "character varying(900)", maxLength: 900, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PhotoGallery", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PhotoUploadRequires",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    PhotoType = table.Column<string>(type: "text", nullable: false),
                    DisplayName = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    IsRequire = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PhotoUploadRequires", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Ptos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    RsaNumber = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Address = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: false),
                    Latitude = table.Column<double>(type: "double precision", nullable: false),
                    Longitude = table.Column<double>(type: "double precision", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Ptos", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "RouteAINeuronNetworks",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    Link = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RouteAINeuronNetworks", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    FIO = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    Email = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    Password = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    RoleType = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    RegDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "VehicleCategoriesByOkp",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Description = table.Column<string>(type: "text", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VehicleCategoriesByOkp", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "VehicleCategoriesBySrts",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Description = table.Column<string>(type: "text", nullable: false),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VehicleCategoriesBySrts", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PtoExperts",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    PtoId = table.Column<int>(type: "integer", nullable: false),
                    FirstName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    LastName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    MiddleName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    SequenceNumber = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PtoExperts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PtoExperts_Ptos_PtoId",
                        column: x => x.PtoId,
                        principalTable: "Ptos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PtoOperatorExpertCryptoKeys",
                columns: table => new
                {
                    PtoId = table.Column<int>(type: "integer", nullable: false),
                    EncryptedData = table.Column<byte[]>(type: "bytea", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PtoOperatorExpertCryptoKeys", x => x.PtoId);
                    table.ForeignKey(
                        name: "FK_PtoOperatorExpertCryptoKeys_Ptos_PtoId",
                        column: x => x.PtoId,
                        principalTable: "Ptos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AdminProfiles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AdminProfiles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AdminProfiles_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ModeratorProfiles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ModeratorProfiles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ModeratorProfiles_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Notifications",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<int>(type: "integer", nullable: false),
                    Title = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    Message = table.Column<string>(type: "text", nullable: false),
                    NotificationType = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    IsRead = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Notifications", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Notifications_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RefreshTokens",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Hash = table.Column<string>(type: "text", nullable: false),
                    UserId = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ExpiresAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RefreshTokens", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RefreshTokens_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserBlocks",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<int>(type: "integer", nullable: false),
                    BlockedByUserId = table.Column<int>(type: "integer", nullable: false),
                    Reason = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    BlockedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    BlockedUntil = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    UnblockedByUserId = table.Column<int>(type: "integer", nullable: true),
                    UnblockedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    UnblockReason = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserBlocks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserBlocks_Users_BlockedByUserId",
                        column: x => x.BlockedByUserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_UserBlocks_Users_UnblockedByUserId",
                        column: x => x.UnblockedByUserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_UserBlocks_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserStatuses",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<int>(type: "integer", nullable: false),
                    IsBlocked = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    BlockedUntil = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserStatuses", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserStatuses_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PtoPolicies",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    PtoId = table.Column<int>(type: "integer", nullable: false),
                    VehicleCategoryBySrtsId = table.Column<int>(type: "integer", nullable: false),
                    Price = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PtoPolicies", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PtoPolicies_Ptos_PtoId",
                        column: x => x.PtoId,
                        principalTable: "Ptos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PtoPolicies_VehicleCategoriesBySrts_VehicleCategoryBySrtsId",
                        column: x => x.VehicleCategoryBySrtsId,
                        principalTable: "VehicleCategoriesBySrts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PtoExpertCryptoKeys",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ExpertId = table.Column<int>(type: "integer", nullable: false),
                    EncryptedData = table.Column<byte[]>(type: "bytea", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PtoExpertCryptoKeys", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PtoExpertCryptoKeys_PtoExperts_ExpertId",
                        column: x => x.ExpertId,
                        principalTable: "PtoExperts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Prompts",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<int>(type: "integer", nullable: false),
                    Tag = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: false),
                    AdminProfileId = table.Column<int>(type: "integer", nullable: true),
                    ModeratorProfileId = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Prompts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Prompts_AdminProfiles_AdminProfileId",
                        column: x => x.AdminProfileId,
                        principalTable: "AdminProfiles",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Prompts_ModeratorProfiles_ModeratorProfileId",
                        column: x => x.ModeratorProfileId,
                        principalTable: "ModeratorProfiles",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Prompts_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AgentBranches",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Fee = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    OwnerAgentId = table.Column<int>(type: "integer", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AgentBranches", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AgentProfiles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<int>(type: "integer", nullable: false),
                    ParentAgentId = table.Column<int>(type: "integer", nullable: true),
                    Path = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    BranchId = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AgentProfiles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AgentProfiles_AgentBranches_BranchId",
                        column: x => x.BranchId,
                        principalTable: "AgentBranches",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_AgentProfiles_AgentProfiles_ParentAgentId",
                        column: x => x.ParentAgentId,
                        principalTable: "AgentProfiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AgentProfiles_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserInvitations",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Email = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    RoleType = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    Token = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    ExpiresAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedByUserId = table.Column<int>(type: "integer", nullable: false),
                    IsUsed = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    UsedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    BranchId = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserInvitations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserInvitations_AgentBranches_BranchId",
                        column: x => x.BranchId,
                        principalTable: "AgentBranches",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_UserInvitations_Users_CreatedByUserId",
                        column: x => x.CreatedByUserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Applications",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    AgentId = table.Column<int>(type: "integer", nullable: false),
                    ProvidedDocumentId = table.Column<int>(type: "integer", nullable: false),
                    ProvidedDocumentSeriesAndNumber = table.Column<string>(type: "text", nullable: false),
                    ProvidedDocumentWhenWasIssued = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ProvidedDocumentIssuedBy = table.Column<string>(type: "text", nullable: false),
                    IsVinMissing = table.Column<bool>(type: "boolean", nullable: false),
                    VIN = table.Column<string>(type: "character varying(17)", maxLength: 17, nullable: false),
                    VehicleYearOfRelease = table.Column<int>(type: "integer", nullable: false),
                    Brand = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Model = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    GosNumber = table.Column<string>(type: "character varying(15)", maxLength: 15, nullable: false),
                    VehicleBodyNumber = table.Column<string>(type: "text", nullable: false),
                    IsChassisNumberMissing = table.Column<bool>(type: "boolean", nullable: false),
                    ChassisNumber = table.Column<string>(type: "text", nullable: true),
                    AdditionalMarking = table.Column<int>(type: "integer", nullable: true),
                    MassWithoutLoad = table.Column<int>(type: "integer", nullable: false),
                    VehicleCategoryBySrtsId = table.Column<int>(type: "integer", nullable: false),
                    VehicleCategoryByOkpId = table.Column<int>(type: "integer", nullable: false),
                    VehicleMileage = table.Column<int>(type: "integer", nullable: false),
                    AlowedMaximumMass = table.Column<int>(type: "integer", nullable: false),
                    FuelType = table.Column<string>(type: "text", nullable: false),
                    TypeOfBrakeSystemDrive = table.Column<string>(type: "text", nullable: false),
                    TireBrand = table.Column<string>(type: "text", nullable: false),
                    SpecialPurpose = table.Column<string>(type: "text", nullable: true),
                    IsTahogramMissing = table.Column<bool>(type: "boolean", nullable: false),
                    BrandOfTachograph = table.Column<string>(type: "text", nullable: true),
                    ModelOfTachograph = table.Column<string>(type: "text", nullable: true),
                    SerialNumberOfTachograph = table.Column<string>(type: "text", nullable: true),
                    IsGasCylinderEquipmentMissing = table.Column<bool>(type: "boolean", nullable: false),
                    NumberOfGasCylinderEquipment = table.Column<string>(type: "text", nullable: true),
                    DateOfTheNextInspectionGasCylinderEquipment = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsGasCylinderMissing = table.Column<bool>(type: "boolean", nullable: false),
                    GasCylinderYearOfRelease = table.Column<int>(type: "integer", nullable: true),
                    GasCylinderSerialNumber = table.Column<string>(type: "text", nullable: true),
                    DateOfTheLastExaminationGasCylinder = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    DateOfTheNextInspectionGasCylinder = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    FIO = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    Email = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    PhoneNumber = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    PtoId = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Status = table.Column<string>(type: "text", nullable: false),
                    RejectReason = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Applications", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Applications_AgentProfiles_AgentId",
                        column: x => x.AgentId,
                        principalTable: "AgentProfiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Applications_ApplicationProvidedDocuments_ProvidedDocumentId",
                        column: x => x.ProvidedDocumentId,
                        principalTable: "ApplicationProvidedDocuments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Applications_Ptos_PtoId",
                        column: x => x.PtoId,
                        principalTable: "Ptos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Applications_VehicleCategoriesByOkp_VehicleCategoryByOkpId",
                        column: x => x.VehicleCategoryByOkpId,
                        principalTable: "VehicleCategoriesByOkp",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Applications_VehicleCategoriesBySrts_VehicleCategoryBySrtsId",
                        column: x => x.VehicleCategoryBySrtsId,
                        principalTable: "VehicleCategoriesBySrts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Wallets",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    AgentId = table.Column<int>(type: "integer", nullable: false),
                    Balance = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    DebtLimit = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    CurrentDebt = table.Column<decimal>(type: "numeric", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Wallets", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Wallets_AgentProfiles_AgentId",
                        column: x => x.AgentId,
                        principalTable: "AgentProfiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ApplicationPhotos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ApplicationId = table.Column<int>(type: "integer", nullable: false),
                    VehiclePhotoType = table.Column<string>(type: "text", nullable: false),
                    Url = table.Column<string>(type: "character varying(2048)", maxLength: 2048, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApplicationPhotos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ApplicationPhotos_Applications_ApplicationId",
                        column: x => x.ApplicationId,
                        principalTable: "Applications",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Transactions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ExternalId = table.Column<Guid>(type: "uuid", nullable: false),
                    IdempotencyKey = table.Column<Guid>(type: "uuid", nullable: false),
                    WalletId = table.Column<int>(type: "integer", nullable: false),
                    Amount = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    TransactionType = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    TransactionStatus = table.Column<string>(type: "text", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Transactions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Transactions_Wallets_WalletId",
                        column: x => x.WalletId,
                        principalTable: "Wallets",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AdminProfiles_UserId",
                table: "AdminProfiles",
                column: "UserId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AgentBranches_OwnerAgentId",
                table: "AgentBranches",
                column: "OwnerAgentId");

            migrationBuilder.CreateIndex(
                name: "IX_AgentProfiles_BranchId",
                table: "AgentProfiles",
                column: "BranchId");

            migrationBuilder.CreateIndex(
                name: "IX_AgentProfiles_ParentAgentId",
                table: "AgentProfiles",
                column: "ParentAgentId");

            migrationBuilder.CreateIndex(
                name: "IX_AgentProfiles_Path",
                table: "AgentProfiles",
                column: "Path");

            migrationBuilder.CreateIndex(
                name: "IX_AgentProfiles_UserId",
                table: "AgentProfiles",
                column: "UserId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ApplicationPhotos_ApplicationId",
                table: "ApplicationPhotos",
                column: "ApplicationId");

            migrationBuilder.CreateIndex(
                name: "IX_ApplicationPhotos_VehiclePhotoType",
                table: "ApplicationPhotos",
                column: "VehiclePhotoType");

            migrationBuilder.CreateIndex(
                name: "IX_Applications_AgentId",
                table: "Applications",
                column: "AgentId");

            migrationBuilder.CreateIndex(
                name: "IX_Applications_Email",
                table: "Applications",
                column: "Email");

            migrationBuilder.CreateIndex(
                name: "IX_Applications_GosNumber",
                table: "Applications",
                column: "GosNumber");

            migrationBuilder.CreateIndex(
                name: "IX_Applications_ProvidedDocumentId",
                table: "Applications",
                column: "ProvidedDocumentId");

            migrationBuilder.CreateIndex(
                name: "IX_Applications_PtoId",
                table: "Applications",
                column: "PtoId");

            migrationBuilder.CreateIndex(
                name: "IX_Applications_VehicleCategoryByOkpId",
                table: "Applications",
                column: "VehicleCategoryByOkpId");

            migrationBuilder.CreateIndex(
                name: "IX_Applications_VehicleCategoryBySrtsId",
                table: "Applications",
                column: "VehicleCategoryBySrtsId");

            migrationBuilder.CreateIndex(
                name: "IX_Applications_VIN",
                table: "Applications",
                column: "VIN");

            migrationBuilder.CreateIndex(
                name: "IX_ModeratorProfiles_UserId",
                table: "ModeratorProfiles",
                column: "UserId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_UserId",
                table: "Notifications",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_PhotoGallery_CreatedAt",
                table: "PhotoGallery",
                column: "CreatedAt",
                descending: new bool[0]);

            migrationBuilder.CreateIndex(
                name: "IX_PhotoGallery_Group",
                table: "PhotoGallery",
                column: "Group");

            migrationBuilder.CreateIndex(
                name: "IX_PhotoGallery_PublicId",
                table: "PhotoGallery",
                column: "PublicId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PhotoGallery_Tag",
                table: "PhotoGallery",
                column: "Tag");

            migrationBuilder.CreateIndex(
                name: "IX_Prompts_AdminProfileId",
                table: "Prompts",
                column: "AdminProfileId");

            migrationBuilder.CreateIndex(
                name: "IX_Prompts_ModeratorProfileId",
                table: "Prompts",
                column: "ModeratorProfileId");

            migrationBuilder.CreateIndex(
                name: "IX_Prompts_UserId",
                table: "Prompts",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "UX_PtoExpertCryptoKeys_ExpertId",
                table: "PtoExpertCryptoKeys",
                column: "ExpertId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PtoExperts_PtoId",
                table: "PtoExperts",
                column: "PtoId");

            migrationBuilder.CreateIndex(
                name: "IX_PtoPolicies_PtoId_VehicleCategoryBySrtsId",
                table: "PtoPolicies",
                columns: new[] { "PtoId", "VehicleCategoryBySrtsId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PtoPolicies_VehicleCategoryBySrtsId",
                table: "PtoPolicies",
                column: "VehicleCategoryBySrtsId");

            migrationBuilder.CreateIndex(
                name: "IX_RefreshTokens_Hash",
                table: "RefreshTokens",
                column: "Hash",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RefreshTokens_UserId",
                table: "RefreshTokens",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Transactions_CreatedAt",
                table: "Transactions",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_Transactions_IdempotencyKey",
                table: "Transactions",
                column: "IdempotencyKey",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Transactions_WalletId",
                table: "Transactions",
                column: "WalletId");

            migrationBuilder.CreateIndex(
                name: "IX_UserBlocks_BlockedAt",
                table: "UserBlocks",
                column: "BlockedAt");

            migrationBuilder.CreateIndex(
                name: "IX_UserBlocks_BlockedByUserId",
                table: "UserBlocks",
                column: "BlockedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserBlocks_UnblockedByUserId",
                table: "UserBlocks",
                column: "UnblockedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserBlocks_UserId",
                table: "UserBlocks",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserBlocks_UserId_UnblockedAt",
                table: "UserBlocks",
                columns: new[] { "UserId", "UnblockedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_UserInvitations_BranchId",
                table: "UserInvitations",
                column: "BranchId");

            migrationBuilder.CreateIndex(
                name: "IX_UserInvitations_CreatedByUserId",
                table: "UserInvitations",
                column: "CreatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserInvitations_Email",
                table: "UserInvitations",
                column: "Email");

            migrationBuilder.CreateIndex(
                name: "IX_UserInvitations_Token",
                table: "UserInvitations",
                column: "Token",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_Email",
                table: "Users",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserStatuses_IsBlocked",
                table: "UserStatuses",
                column: "IsBlocked");

            migrationBuilder.CreateIndex(
                name: "IX_UserStatuses_UserId",
                table: "UserStatuses",
                column: "UserId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_VehicleCategoriesBySrts_Name",
                table: "VehicleCategoriesBySrts",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Wallets_AgentId",
                table: "Wallets",
                column: "AgentId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_AgentBranches_AgentProfiles_OwnerAgentId",
                table: "AgentBranches",
                column: "OwnerAgentId",
                principalTable: "AgentProfiles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AgentProfiles_Users_UserId",
                table: "AgentProfiles");

            migrationBuilder.DropForeignKey(
                name: "FK_AgentBranches_AgentProfiles_OwnerAgentId",
                table: "AgentBranches");

            migrationBuilder.DropTable(
                name: "ApplicationPhotos");

            migrationBuilder.DropTable(
                name: "Notifications");

            migrationBuilder.DropTable(
                name: "PhotoGallery");

            migrationBuilder.DropTable(
                name: "PhotoUploadRequires");

            migrationBuilder.DropTable(
                name: "Prompts");

            migrationBuilder.DropTable(
                name: "PtoExpertCryptoKeys");

            migrationBuilder.DropTable(
                name: "PtoOperatorExpertCryptoKeys");

            migrationBuilder.DropTable(
                name: "PtoPolicies");

            migrationBuilder.DropTable(
                name: "RefreshTokens");

            migrationBuilder.DropTable(
                name: "RouteAINeuronNetworks");

            migrationBuilder.DropTable(
                name: "Transactions");

            migrationBuilder.DropTable(
                name: "UserBlocks");

            migrationBuilder.DropTable(
                name: "UserInvitations");

            migrationBuilder.DropTable(
                name: "UserStatuses");

            migrationBuilder.DropTable(
                name: "Applications");

            migrationBuilder.DropTable(
                name: "AdminProfiles");

            migrationBuilder.DropTable(
                name: "ModeratorProfiles");

            migrationBuilder.DropTable(
                name: "PtoExperts");

            migrationBuilder.DropTable(
                name: "Wallets");

            migrationBuilder.DropTable(
                name: "ApplicationProvidedDocuments");

            migrationBuilder.DropTable(
                name: "VehicleCategoriesByOkp");

            migrationBuilder.DropTable(
                name: "VehicleCategoriesBySrts");

            migrationBuilder.DropTable(
                name: "Ptos");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "AgentProfiles");

            migrationBuilder.DropTable(
                name: "AgentBranches");
        }
    }
}
