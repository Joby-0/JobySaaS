using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace DbContext.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "SubscriptionPlans",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    Name = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    StripePriceId = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Price = table.Column<int>(type: "int", nullable: false),
                    BillingIntervalInMonths = table.Column<int>(type: "int", nullable: false),
                    isFree = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    ContactSales = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    IsActive = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    Description = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SubscriptionPlans", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    UserName = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Email = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "FeatureDbM",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    SubscriptionPlanId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    Name = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FeatureDbM", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FeatureDbM_SubscriptionPlans_SubscriptionPlanId",
                        column: x => x.SubscriptionPlanId,
                        principalTable: "SubscriptionPlans",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Media",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    OrganizationId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    FileUrl = table.Column<string>(type: "varchar(256)", maxLength: 256, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ThumbnailUrl = table.Column<string>(type: "varchar(256)", maxLength: 256, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Title = table.Column<string>(type: "varchar(120)", maxLength: 120, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Description = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Duration = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Media", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "OrganizationInvitations",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    InvitedByUserId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    OrganizationId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    InvitedEmail = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Role = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    InviteCode = table.Column<string>(type: "varchar(255)", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ExpiresAt = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    AcceptedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    IsActive = table.Column<bool>(type: "tinyint(1)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrganizationInvitations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OrganizationInvitations_Users_InvitedByUserId",
                        column: x => x.InvitedByUserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Organizations",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    OrganizationSubscriptionId = table.Column<Guid>(type: "char(36)", nullable: true, collation: "ascii_general_ci"),
                    Name = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    OwnerId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Organizations", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "OrganizationSubscriptions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    OrganizationId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    SubscriptionPlanId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    Status = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    StripeSubscriptionId = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CurrentPeriodStart = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    CurrentPeriodEnd = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    CancelAtPeriodEnd = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    StripeCustomerId = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrganizationSubscriptions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OrganizationSubscriptions_Organizations_OrganizationId",
                        column: x => x.OrganizationId,
                        principalTable: "Organizations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OrganizationSubscriptions_SubscriptionPlans_SubscriptionPlan~",
                        column: x => x.SubscriptionPlanId,
                        principalTable: "SubscriptionPlans",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "SocialAccounts",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    OrganizationId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    Platform = table.Column<int>(type: "int", nullable: false),
                    Username = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    AccessToken = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    RefreshToken = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    TokenExpiresAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    CostumUrl = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ProfileImageUrl = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    LastSync = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    Followers = table.Column<ulong>(type: "bigint unsigned", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SocialAccounts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SocialAccounts_Organizations_OrganizationId",
                        column: x => x.OrganizationId,
                        principalTable: "Organizations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "UserOrganizations",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    UserId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    OrganizationId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    Role = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserOrganizations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserOrganizations_Organizations_OrganizationId",
                        column: x => x.OrganizationId,
                        principalTable: "Organizations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserOrganizations_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Posts",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    MediaId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    SocialAccountId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    Status = table.Column<string>(type: "varchar(120)", maxLength: 120, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ScheduledAt = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    PublishedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    PlatformPostId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    OrganizationDbMId = table.Column<Guid>(type: "char(36)", nullable: true, collation: "ascii_general_ci")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Posts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Posts_Media_MediaId",
                        column: x => x.MediaId,
                        principalTable: "Media",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Posts_Organizations_OrganizationDbMId",
                        column: x => x.OrganizationDbMId,
                        principalTable: "Organizations",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Posts_SocialAccounts_SocialAccountId",
                        column: x => x.SocialAccountId,
                        principalTable: "SocialAccounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "PostAnalytics",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    PostId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    Views = table.Column<int>(type: "int", nullable: false),
                    Likes = table.Column<int>(type: "int", nullable: false),
                    Comments = table.Column<int>(type: "int", nullable: false),
                    Shares = table.Column<int>(type: "int", nullable: false),
                    ReportedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PostAnalytics", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PostAnalytics_Posts_PostId",
                        column: x => x.PostId,
                        principalTable: "Posts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.InsertData(
                table: "SubscriptionPlans",
                columns: new[] { "Id", "BillingIntervalInMonths", "ContactSales", "Description", "IsActive", "Name", "Price", "StripePriceId", "isFree" },
                values: new object[,]
                {
                    { new Guid("11111111-1111-1111-1111-111111111111"), 1, false, null, true, "Free", 0, null, true },
                    { new Guid("22222222-2222-2222-2222-222222222222"), 1, false, null, true, "Basic", 10, "price_1U8eLiBD6lDY7COkLzS0Q7Vx", false },
                    { new Guid("33333333-3333-3333-3333-333333333333"), 1, false, null, true, "Pro", 25, "price_1U3MahBD6lDY7COkvKIK29cg", false },
                    { new Guid("44444444-4444-4444-4444-444444444444"), 1, true, null, true, "Enterprise", 0, null, false },
                    { new Guid("55555555-5555-5555-5555-555555555555"), 12, false, null, true, "Basic", 100, "price_1U8eMABD6lDY7COkoje7yEBw", false },
                    { new Guid("66666666-6666-6666-6666-666666666666"), 12, false, null, true, "Pro", 250, "price_1U3N6rBD6lDY7COkO6ixkp2H", false }
                });

            migrationBuilder.InsertData(
                table: "FeatureDbM",
                columns: new[] { "Id", "Name", "SubscriptionPlanId" },
                values: new object[,]
                {
                    { new Guid("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"), "1 user", new Guid("11111111-1111-1111-1111-111111111111") },
                    { new Guid("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaab"), "1 social account", new Guid("11111111-1111-1111-1111-111111111111") },
                    { new Guid("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaac"), "Limited uploads", new Guid("11111111-1111-1111-1111-111111111111") },
                    { new Guid("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaad"), "Basic analytics", new Guid("11111111-1111-1111-1111-111111111111") },
                    { new Guid("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbaa"), "Up to 5 users", new Guid("22222222-2222-2222-2222-222222222222") },
                    { new Guid("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbab"), "Up to 5 social accounts", new Guid("22222222-2222-2222-2222-222222222222") },
                    { new Guid("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbac"), "Unlimited uploads", new Guid("22222222-2222-2222-2222-222222222222") },
                    { new Guid("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbad"), "Basic analytics", new Guid("22222222-2222-2222-2222-222222222222") },
                    { new Guid("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbae"), "Scheduled publishing", new Guid("22222222-2222-2222-2222-222222222222") },
                    { new Guid("cccccccc-cccc-cccc-cccc-ccccccccccca"), "Up to 15 users", new Guid("33333333-3333-3333-3333-333333333333") },
                    { new Guid("cccccccc-cccc-cccc-cccc-cccccccccccb"), "Up to 20 social accounts", new Guid("33333333-3333-3333-3333-333333333333") },
                    { new Guid("cccccccc-cccc-cccc-cccc-cccccccccccc"), "Publish to X (Twitter)", new Guid("33333333-3333-3333-3333-333333333333") },
                    { new Guid("cccccccc-cccc-cccc-cccc-cccccccccccd"), "Unlimited uploads", new Guid("33333333-3333-3333-3333-333333333333") },
                    { new Guid("cccccccc-cccc-cccc-cccc-ccccccccccce"), "Advanced analytics", new Guid("33333333-3333-3333-3333-333333333333") },
                    { new Guid("cccccccc-cccc-cccc-cccc-cccccccccccf"), "Scheduled publishing", new Guid("33333333-3333-3333-3333-333333333333") },
                    { new Guid("cccccccc-cccc-cccc-cccc-ccccccccccd0"), "Content management", new Guid("33333333-3333-3333-3333-333333333333") },
                    { new Guid("cccccccc-cccc-cccc-cccc-ccccccccccd1"), "Team collaboration", new Guid("33333333-3333-3333-3333-333333333333") },
                    { new Guid("dddddddd-dddd-dddd-dddd-ddddddddddda"), "Unlimited users", new Guid("44444444-4444-4444-4444-444444444444") },
                    { new Guid("dddddddd-dddd-dddd-dddd-dddddddddddb"), "Unlimited social accounts", new Guid("44444444-4444-4444-4444-444444444444") },
                    { new Guid("dddddddd-dddd-dddd-dddd-dddddddddddc"), "Publish to X (Twitter)", new Guid("44444444-4444-4444-4444-444444444444") },
                    { new Guid("dddddddd-dddd-dddd-dddd-dddddddddddd"), "Unlimited uploads", new Guid("44444444-4444-4444-4444-444444444444") },
                    { new Guid("dddddddd-dddd-dddd-dddd-ddddddddddde"), "Advanced analytics", new Guid("44444444-4444-4444-4444-444444444444") },
                    { new Guid("dddddddd-dddd-dddd-dddd-dddddddddddf"), "Scheduled publishing", new Guid("44444444-4444-4444-4444-444444444444") },
                    { new Guid("dddddddd-dddd-dddd-dddd-ddddddddddf0"), "Team collaboration", new Guid("44444444-4444-4444-4444-444444444444") },
                    { new Guid("dddddddd-dddd-dddd-dddd-ddddddddddf1"), "Priority support", new Guid("44444444-4444-4444-4444-444444444444") },
                    { new Guid("dddddddd-dddd-dddd-dddd-ddddddddddf2"), "Custom integrations", new Guid("44444444-4444-4444-4444-444444444444") },
                    { new Guid("dddddddd-dddd-dddd-dddd-ddddddddddf3"), "Dedicated support", new Guid("44444444-4444-4444-4444-444444444444") }
                });

            migrationBuilder.CreateIndex(
                name: "IX_FeatureDbM_SubscriptionPlanId",
                table: "FeatureDbM",
                column: "SubscriptionPlanId");

            migrationBuilder.CreateIndex(
                name: "IX_Media_OrganizationId",
                table: "Media",
                column: "OrganizationId");

            migrationBuilder.CreateIndex(
                name: "IX_OrganizationInvitations_InviteCode",
                table: "OrganizationInvitations",
                column: "InviteCode",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_OrganizationInvitations_InvitedByUserId",
                table: "OrganizationInvitations",
                column: "InvitedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_OrganizationInvitations_OrganizationId",
                table: "OrganizationInvitations",
                column: "OrganizationId");

            migrationBuilder.CreateIndex(
                name: "IX_Organizations_OrganizationSubscriptionId",
                table: "Organizations",
                column: "OrganizationSubscriptionId");

            migrationBuilder.CreateIndex(
                name: "IX_OrganizationSubscriptions_OrganizationId",
                table: "OrganizationSubscriptions",
                column: "OrganizationId");

            migrationBuilder.CreateIndex(
                name: "IX_OrganizationSubscriptions_SubscriptionPlanId",
                table: "OrganizationSubscriptions",
                column: "SubscriptionPlanId");

            migrationBuilder.CreateIndex(
                name: "IX_PostAnalytics_PostId",
                table: "PostAnalytics",
                column: "PostId");

            migrationBuilder.CreateIndex(
                name: "IX_Posts_MediaId",
                table: "Posts",
                column: "MediaId");

            migrationBuilder.CreateIndex(
                name: "IX_Posts_OrganizationDbMId",
                table: "Posts",
                column: "OrganizationDbMId");

            migrationBuilder.CreateIndex(
                name: "IX_Posts_SocialAccountId",
                table: "Posts",
                column: "SocialAccountId");

            migrationBuilder.CreateIndex(
                name: "IX_SocialAccounts_OrganizationId",
                table: "SocialAccounts",
                column: "OrganizationId");

            migrationBuilder.CreateIndex(
                name: "IX_UserOrganizations_OrganizationId",
                table: "UserOrganizations",
                column: "OrganizationId");

            migrationBuilder.CreateIndex(
                name: "IX_UserOrganizations_UserId",
                table: "UserOrganizations",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Media_Organizations_OrganizationId",
                table: "Media",
                column: "OrganizationId",
                principalTable: "Organizations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_OrganizationInvitations_Organizations_OrganizationId",
                table: "OrganizationInvitations",
                column: "OrganizationId",
                principalTable: "Organizations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Organizations_OrganizationSubscriptions_OrganizationSubscrip~",
                table: "Organizations",
                column: "OrganizationSubscriptionId",
                principalTable: "OrganizationSubscriptions",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OrganizationSubscriptions_SubscriptionPlans_SubscriptionPlan~",
                table: "OrganizationSubscriptions");

            migrationBuilder.DropForeignKey(
                name: "FK_OrganizationSubscriptions_Organizations_OrganizationId",
                table: "OrganizationSubscriptions");

            migrationBuilder.DropTable(
                name: "FeatureDbM");

            migrationBuilder.DropTable(
                name: "OrganizationInvitations");

            migrationBuilder.DropTable(
                name: "PostAnalytics");

            migrationBuilder.DropTable(
                name: "UserOrganizations");

            migrationBuilder.DropTable(
                name: "Posts");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "Media");

            migrationBuilder.DropTable(
                name: "SocialAccounts");

            migrationBuilder.DropTable(
                name: "SubscriptionPlans");

            migrationBuilder.DropTable(
                name: "Organizations");

            migrationBuilder.DropTable(
                name: "OrganizationSubscriptions");
        }
    }
}
