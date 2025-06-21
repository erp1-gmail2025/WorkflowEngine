using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WorkflowAPI.Migrations
{
    /// <inheritdoc />
    public partial class InitData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ActionTypes",
                columns: table => new
                {
                    ActionTypeID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ActionTypes", x => x.ActionTypeID);
                });

            migrationBuilder.CreateTable(
                name: "ActivityTypes",
                columns: table => new
                {
                    ActivityTypeID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ActivityTypes", x => x.ActivityTypeID);
                });

            migrationBuilder.CreateTable(
                name: "Groups",
                columns: table => new
                {
                    GroupID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Groups", x => x.GroupID);
                });

            migrationBuilder.CreateTable(
                name: "Processes",
                columns: table => new
                {
                    ProcessID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Processes", x => x.ProcessID);
                });

            migrationBuilder.CreateTable(
                name: "StateTypes",
                columns: table => new
                {
                    StateTypeID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Key = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StateTypes", x => x.StateTypeID);
                });

            migrationBuilder.CreateTable(
                name: "Targets",
                columns: table => new
                {
                    TargetID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Targets", x => x.TargetID);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    UserID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FirstName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LastName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DateOfBirth = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.UserID);
                });

            migrationBuilder.CreateTable(
                name: "Activities",
                columns: table => new
                {
                    ActivityID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ActivityTypeID = table.Column<int>(type: "int", nullable: false),
                    ProcessID = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Activities", x => x.ActivityID);
                    table.ForeignKey(
                        name: "FK_Activities_ActivityTypes_ActivityTypeID",
                        column: x => x.ActivityTypeID,
                        principalTable: "ActivityTypes",
                        principalColumn: "ActivityTypeID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Activities_Processes_ProcessID",
                        column: x => x.ProcessID,
                        principalTable: "Processes",
                        principalColumn: "ProcessID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CustomEntities",
                columns: table => new
                {
                    CustomEntityID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EntityType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Data = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ProcessID = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CustomEntities", x => x.CustomEntityID);
                    table.ForeignKey(
                        name: "FK_CustomEntities_Processes_ProcessID",
                        column: x => x.ProcessID,
                        principalTable: "Processes",
                        principalColumn: "ProcessID");
                });

            migrationBuilder.CreateTable(
                name: "WorkflowActions",
                columns: table => new
                {
                    WorkflowActionID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ActionTypeID = table.Column<int>(type: "int", nullable: false),
                    ProcessID = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WorkflowActions", x => x.WorkflowActionID);
                    table.ForeignKey(
                        name: "FK_WorkflowActions_ActionTypes_ActionTypeID",
                        column: x => x.ActionTypeID,
                        principalTable: "ActionTypes",
                        principalColumn: "ActionTypeID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_WorkflowActions_Processes_ProcessID",
                        column: x => x.ProcessID,
                        principalTable: "Processes",
                        principalColumn: "ProcessID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "States",
                columns: table => new
                {
                    StateID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProcessID = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    StateTypeID = table.Column<int>(type: "int", nullable: false),
                    IsFinal = table.Column<bool>(type: "bit", nullable: false),
                    StateOrder = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_States", x => x.StateID);
                    table.ForeignKey(
                        name: "FK_States_Processes_ProcessID",
                        column: x => x.ProcessID,
                        principalTable: "Processes",
                        principalColumn: "ProcessID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_States_StateTypes_StateTypeID",
                        column: x => x.StateTypeID,
                        principalTable: "StateTypes",
                        principalColumn: "StateTypeID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "GroupMembers",
                columns: table => new
                {
                    UserID = table.Column<int>(type: "int", nullable: false),
                    GroupID = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GroupMembers", x => new { x.UserID, x.GroupID });
                    table.ForeignKey(
                        name: "FK_GroupMembers_Groups_GroupID",
                        column: x => x.GroupID,
                        principalTable: "Groups",
                        principalColumn: "GroupID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_GroupMembers_Users_UserID",
                        column: x => x.UserID,
                        principalTable: "Users",
                        principalColumn: "UserID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProcessAdmins",
                columns: table => new
                {
                    ProcessID = table.Column<int>(type: "int", nullable: false),
                    UserID = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProcessAdmins", x => new { x.ProcessID, x.UserID });
                    table.ForeignKey(
                        name: "FK_ProcessAdmins_Processes_ProcessID",
                        column: x => x.ProcessID,
                        principalTable: "Processes",
                        principalColumn: "ProcessID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ProcessAdmins_Users_UserID",
                        column: x => x.UserID,
                        principalTable: "Users",
                        principalColumn: "UserID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ActivityTargets",
                columns: table => new
                {
                    ActivityID = table.Column<int>(type: "int", nullable: false),
                    TargetID = table.Column<int>(type: "int", nullable: false),
                    GroupID = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ActivityTargets", x => new { x.ActivityID, x.TargetID, x.GroupID });
                    table.ForeignKey(
                        name: "FK_ActivityTargets_Activities_ActivityID",
                        column: x => x.ActivityID,
                        principalTable: "Activities",
                        principalColumn: "ActivityID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ActivityTargets_Groups_GroupID",
                        column: x => x.GroupID,
                        principalTable: "Groups",
                        principalColumn: "GroupID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ActivityTargets_Targets_TargetID",
                        column: x => x.TargetID,
                        principalTable: "Targets",
                        principalColumn: "TargetID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ActionTargets",
                columns: table => new
                {
                    ActionID = table.Column<int>(type: "int", nullable: false),
                    TargetID = table.Column<int>(type: "int", nullable: false),
                    GroupID = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ActionTargets", x => new { x.ActionID, x.TargetID, x.GroupID });
                    table.ForeignKey(
                        name: "FK_ActionTargets_Groups_GroupID",
                        column: x => x.GroupID,
                        principalTable: "Groups",
                        principalColumn: "GroupID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ActionTargets_Targets_TargetID",
                        column: x => x.TargetID,
                        principalTable: "Targets",
                        principalColumn: "TargetID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ActionTargets_WorkflowActions_ActionID",
                        column: x => x.ActionID,
                        principalTable: "WorkflowActions",
                        principalColumn: "WorkflowActionID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "FieldDefinition",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FieldType = table.Column<int>(type: "int", nullable: false),
                    Label = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    StateId = table.Column<int>(type: "int", nullable: false),
                    Config = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Validation = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FieldDefinition", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FieldDefinition_States_StateId",
                        column: x => x.StateId,
                        principalTable: "States",
                        principalColumn: "StateID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Requests",
                columns: table => new
                {
                    RequestID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DateRequested = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UserID = table.Column<int>(type: "int", nullable: true),
                    ProcessId = table.Column<int>(type: "int", nullable: false),
                    CurrentStateID = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Requests", x => x.RequestID);
                    table.ForeignKey(
                        name: "FK_Requests_Processes_ProcessId",
                        column: x => x.ProcessId,
                        principalTable: "Processes",
                        principalColumn: "ProcessID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Requests_States_CurrentStateID",
                        column: x => x.CurrentStateID,
                        principalTable: "States",
                        principalColumn: "StateID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Requests_Users_UserID",
                        column: x => x.UserID,
                        principalTable: "Users",
                        principalColumn: "UserID");
                });

            migrationBuilder.CreateTable(
                name: "Transitions",
                columns: table => new
                {
                    TransitionID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProcessID = table.Column<int>(type: "int", nullable: false),
                    CurrentStateID = table.Column<int>(type: "int", nullable: false),
                    NextStateID = table.Column<int>(type: "int", nullable: false),
                    IsFinal = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Transitions", x => x.TransitionID);
                    table.ForeignKey(
                        name: "FK_Transitions_Processes_ProcessID",
                        column: x => x.ProcessID,
                        principalTable: "Processes",
                        principalColumn: "ProcessID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Transitions_States_CurrentStateID",
                        column: x => x.CurrentStateID,
                        principalTable: "States",
                        principalColumn: "StateID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Transitions_States_NextStateID",
                        column: x => x.NextStateID,
                        principalTable: "States",
                        principalColumn: "StateID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "RequestData",
                columns: table => new
                {
                    RequestDataID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RequestID = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RequestData", x => x.RequestDataID);
                    table.ForeignKey(
                        name: "FK_RequestData_Requests_RequestID",
                        column: x => x.RequestID,
                        principalTable: "Requests",
                        principalColumn: "RequestID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RequestFiles",
                columns: table => new
                {
                    RequestFileID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RequestID = table.Column<int>(type: "int", nullable: false),
                    UserID = table.Column<int>(type: "int", nullable: false),
                    FileName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FileContent = table.Column<byte[]>(type: "varbinary(max)", nullable: false),
                    MIMETYPE = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RequestFiles", x => x.RequestFileID);
                    table.ForeignKey(
                        name: "FK_RequestFiles_Requests_RequestID",
                        column: x => x.RequestID,
                        principalTable: "Requests",
                        principalColumn: "RequestID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RequestFiles_Users_UserID",
                        column: x => x.UserID,
                        principalTable: "Users",
                        principalColumn: "UserID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "RequestNotes",
                columns: table => new
                {
                    RequestNoteID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RequestID = table.Column<int>(type: "int", nullable: false),
                    UserID = table.Column<int>(type: "int", nullable: false),
                    Note = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RequestNotes", x => x.RequestNoteID);
                    table.ForeignKey(
                        name: "FK_RequestNotes_Requests_RequestID",
                        column: x => x.RequestID,
                        principalTable: "Requests",
                        principalColumn: "RequestID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RequestNotes_Users_UserID",
                        column: x => x.UserID,
                        principalTable: "Users",
                        principalColumn: "UserID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "RequestStakeholder",
                columns: table => new
                {
                    RequestID = table.Column<int>(type: "int", nullable: false),
                    UserID = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RequestStakeholder", x => new { x.RequestID, x.UserID });
                    table.ForeignKey(
                        name: "FK_RequestStakeholder_Requests_RequestID",
                        column: x => x.RequestID,
                        principalTable: "Requests",
                        principalColumn: "RequestID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RequestStakeholder_Users_UserID",
                        column: x => x.UserID,
                        principalTable: "Users",
                        principalColumn: "UserID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "RequestActions",
                columns: table => new
                {
                    RequestActionID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RequestID = table.Column<int>(type: "int", nullable: false),
                    ActionID = table.Column<int>(type: "int", nullable: false),
                    TransitionID = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    IsComplete = table.Column<bool>(type: "bit", nullable: false),
                    IsFinal = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RequestActions", x => x.RequestActionID);
                    table.ForeignKey(
                        name: "FK_RequestActions_Requests_RequestID",
                        column: x => x.RequestID,
                        principalTable: "Requests",
                        principalColumn: "RequestID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RequestActions_Transitions_TransitionID",
                        column: x => x.TransitionID,
                        principalTable: "Transitions",
                        principalColumn: "TransitionID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RequestActions_WorkflowActions_ActionID",
                        column: x => x.ActionID,
                        principalTable: "WorkflowActions",
                        principalColumn: "WorkflowActionID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TransitionActions",
                columns: table => new
                {
                    TransitionID = table.Column<int>(type: "int", nullable: false),
                    ActionID = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TransitionActions", x => new { x.TransitionID, x.ActionID });
                    table.ForeignKey(
                        name: "FK_TransitionActions_Transitions_TransitionID",
                        column: x => x.TransitionID,
                        principalTable: "Transitions",
                        principalColumn: "TransitionID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TransitionActions_WorkflowActions_ActionID",
                        column: x => x.ActionID,
                        principalTable: "WorkflowActions",
                        principalColumn: "WorkflowActionID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TransitionActivities",
                columns: table => new
                {
                    TransitionID = table.Column<int>(type: "int", nullable: false),
                    ActivityID = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TransitionActivities", x => new { x.TransitionID, x.ActivityID });
                    table.ForeignKey(
                        name: "FK_TransitionActivities_Activities_ActivityID",
                        column: x => x.ActivityID,
                        principalTable: "Activities",
                        principalColumn: "ActivityID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TransitionActivities_Transitions_TransitionID",
                        column: x => x.TransitionID,
                        principalTable: "Transitions",
                        principalColumn: "TransitionID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ActionTargets_GroupID",
                table: "ActionTargets",
                column: "GroupID");

            migrationBuilder.CreateIndex(
                name: "IX_ActionTargets_TargetID",
                table: "ActionTargets",
                column: "TargetID");

            migrationBuilder.CreateIndex(
                name: "IX_Activities_ActivityTypeID",
                table: "Activities",
                column: "ActivityTypeID");

            migrationBuilder.CreateIndex(
                name: "IX_Activities_ProcessID",
                table: "Activities",
                column: "ProcessID");

            migrationBuilder.CreateIndex(
                name: "IX_ActivityTargets_GroupID",
                table: "ActivityTargets",
                column: "GroupID");

            migrationBuilder.CreateIndex(
                name: "IX_ActivityTargets_TargetID",
                table: "ActivityTargets",
                column: "TargetID");

            migrationBuilder.CreateIndex(
                name: "IX_CustomEntities_ProcessID",
                table: "CustomEntities",
                column: "ProcessID");

            migrationBuilder.CreateIndex(
                name: "IX_FieldDefinition_StateId",
                table: "FieldDefinition",
                column: "StateId");

            migrationBuilder.CreateIndex(
                name: "IX_GroupMembers_GroupID",
                table: "GroupMembers",
                column: "GroupID");

            migrationBuilder.CreateIndex(
                name: "IX_ProcessAdmins_UserID",
                table: "ProcessAdmins",
                column: "UserID");

            migrationBuilder.CreateIndex(
                name: "IX_RequestActions_ActionID",
                table: "RequestActions",
                column: "ActionID");

            migrationBuilder.CreateIndex(
                name: "IX_RequestActions_RequestID",
                table: "RequestActions",
                column: "RequestID");

            migrationBuilder.CreateIndex(
                name: "IX_RequestActions_TransitionID",
                table: "RequestActions",
                column: "TransitionID");

            migrationBuilder.CreateIndex(
                name: "IX_RequestData_RequestID",
                table: "RequestData",
                column: "RequestID");

            migrationBuilder.CreateIndex(
                name: "IX_RequestFiles_RequestID",
                table: "RequestFiles",
                column: "RequestID");

            migrationBuilder.CreateIndex(
                name: "IX_RequestFiles_UserID",
                table: "RequestFiles",
                column: "UserID");

            migrationBuilder.CreateIndex(
                name: "IX_RequestNotes_RequestID",
                table: "RequestNotes",
                column: "RequestID");

            migrationBuilder.CreateIndex(
                name: "IX_RequestNotes_UserID",
                table: "RequestNotes",
                column: "UserID");

            migrationBuilder.CreateIndex(
                name: "IX_Requests_CurrentStateID",
                table: "Requests",
                column: "CurrentStateID");

            migrationBuilder.CreateIndex(
                name: "IX_Requests_ProcessId",
                table: "Requests",
                column: "ProcessId");

            migrationBuilder.CreateIndex(
                name: "IX_Requests_UserID",
                table: "Requests",
                column: "UserID");

            migrationBuilder.CreateIndex(
                name: "IX_RequestStakeholder_UserID",
                table: "RequestStakeholder",
                column: "UserID");

            migrationBuilder.CreateIndex(
                name: "IX_States_ProcessID",
                table: "States",
                column: "ProcessID");

            migrationBuilder.CreateIndex(
                name: "IX_States_StateTypeID",
                table: "States",
                column: "StateTypeID");

            migrationBuilder.CreateIndex(
                name: "IX_StateTypes_Key",
                table: "StateTypes",
                column: "Key",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TransitionActions_ActionID",
                table: "TransitionActions",
                column: "ActionID");

            migrationBuilder.CreateIndex(
                name: "IX_TransitionActivities_ActivityID",
                table: "TransitionActivities",
                column: "ActivityID");

            migrationBuilder.CreateIndex(
                name: "IX_Transitions_CurrentStateID",
                table: "Transitions",
                column: "CurrentStateID");

            migrationBuilder.CreateIndex(
                name: "IX_Transitions_NextStateID",
                table: "Transitions",
                column: "NextStateID");

            migrationBuilder.CreateIndex(
                name: "IX_Transitions_ProcessID",
                table: "Transitions",
                column: "ProcessID");

            migrationBuilder.CreateIndex(
                name: "IX_WorkflowActions_ActionTypeID",
                table: "WorkflowActions",
                column: "ActionTypeID");

            migrationBuilder.CreateIndex(
                name: "IX_WorkflowActions_ProcessID",
                table: "WorkflowActions",
                column: "ProcessID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ActionTargets");

            migrationBuilder.DropTable(
                name: "ActivityTargets");

            migrationBuilder.DropTable(
                name: "CustomEntities");

            migrationBuilder.DropTable(
                name: "FieldDefinition");

            migrationBuilder.DropTable(
                name: "GroupMembers");

            migrationBuilder.DropTable(
                name: "ProcessAdmins");

            migrationBuilder.DropTable(
                name: "RequestActions");

            migrationBuilder.DropTable(
                name: "RequestData");

            migrationBuilder.DropTable(
                name: "RequestFiles");

            migrationBuilder.DropTable(
                name: "RequestNotes");

            migrationBuilder.DropTable(
                name: "RequestStakeholder");

            migrationBuilder.DropTable(
                name: "TransitionActions");

            migrationBuilder.DropTable(
                name: "TransitionActivities");

            migrationBuilder.DropTable(
                name: "Targets");

            migrationBuilder.DropTable(
                name: "Groups");

            migrationBuilder.DropTable(
                name: "Requests");

            migrationBuilder.DropTable(
                name: "WorkflowActions");

            migrationBuilder.DropTable(
                name: "Activities");

            migrationBuilder.DropTable(
                name: "Transitions");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "ActionTypes");

            migrationBuilder.DropTable(
                name: "ActivityTypes");

            migrationBuilder.DropTable(
                name: "States");

            migrationBuilder.DropTable(
                name: "Processes");

            migrationBuilder.DropTable(
                name: "StateTypes");
        }
    }
}
