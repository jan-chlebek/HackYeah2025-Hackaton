using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UknfCommunicationPlatform.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AlignWithDatabaseScript : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_announcement_attachments_announcements_AnnouncementId",
                table: "announcement_attachments");

            migrationBuilder.DropForeignKey(
                name: "FK_announcement_histories_announcements_AnnouncementId",
                table: "announcement_histories");

            migrationBuilder.DropForeignKey(
                name: "FK_announcement_histories_users_ChangedByUserId",
                table: "announcement_histories");

            migrationBuilder.DropForeignKey(
                name: "FK_announcement_reads_announcements_AnnouncementId",
                table: "announcement_reads");

            migrationBuilder.DropForeignKey(
                name: "FK_announcement_reads_users_UserId",
                table: "announcement_reads");

            migrationBuilder.DropForeignKey(
                name: "FK_announcement_recipients_announcements_AnnouncementId",
                table: "announcement_recipients");

            migrationBuilder.DropForeignKey(
                name: "FK_announcement_recipients_supervised_entities_SupervisedEntit~",
                table: "announcement_recipients");

            migrationBuilder.DropForeignKey(
                name: "FK_announcement_recipients_users_UserId",
                table: "announcement_recipients");

            migrationBuilder.DropForeignKey(
                name: "FK_announcements_users_CreatedByUserId",
                table: "announcements");

            migrationBuilder.DropForeignKey(
                name: "FK_audit_logs_users_UserId",
                table: "audit_logs");

            migrationBuilder.DropForeignKey(
                name: "FK_case_documents_cases_CaseId",
                table: "case_documents");

            migrationBuilder.DropForeignKey(
                name: "FK_case_documents_users_UploadedByUserId",
                table: "case_documents");

            migrationBuilder.DropForeignKey(
                name: "FK_case_histories_cases_CaseId",
                table: "case_histories");

            migrationBuilder.DropForeignKey(
                name: "FK_case_histories_users_ChangedByUserId",
                table: "case_histories");

            migrationBuilder.DropForeignKey(
                name: "FK_cases_supervised_entities_SupervisedEntityId",
                table: "cases");

            migrationBuilder.DropForeignKey(
                name: "FK_cases_users_CreatedByUserId",
                table: "cases");

            migrationBuilder.DropForeignKey(
                name: "FK_cases_users_HandlerId",
                table: "cases");

            migrationBuilder.DropForeignKey(
                name: "FK_contact_group_members_contact_groups_ContactGroupId",
                table: "contact_group_members");

            migrationBuilder.DropForeignKey(
                name: "FK_contact_group_members_contacts_ContactId",
                table: "contact_group_members");

            migrationBuilder.DropForeignKey(
                name: "FK_contact_groups_users_CreatedByUserId",
                table: "contact_groups");

            migrationBuilder.DropForeignKey(
                name: "FK_contacts_supervised_entities_SupervisedEntityId",
                table: "contacts");

            migrationBuilder.DropForeignKey(
                name: "FK_contacts_users_CreatedByUserId",
                table: "contacts");

            migrationBuilder.DropForeignKey(
                name: "FK_faq_questions_users_AnsweredByUserId",
                table: "faq_questions");

            migrationBuilder.DropForeignKey(
                name: "FK_faq_questions_users_SubmittedByUserId",
                table: "faq_questions");

            migrationBuilder.DropForeignKey(
                name: "FK_faq_ratings_faq_questions_FaqQuestionId",
                table: "faq_ratings");

            migrationBuilder.DropForeignKey(
                name: "FK_faq_ratings_users_UserId",
                table: "faq_ratings");

            migrationBuilder.DropForeignKey(
                name: "FK_file_libraries_file_libraries_ParentFileId",
                table: "file_libraries");

            migrationBuilder.DropForeignKey(
                name: "FK_file_libraries_users_UploadedByUserId",
                table: "file_libraries");

            migrationBuilder.DropForeignKey(
                name: "FK_file_library_permissions_file_libraries_FileLibraryId",
                table: "file_library_permissions");

            migrationBuilder.DropForeignKey(
                name: "FK_file_library_permissions_supervised_entities_SupervisedEnti~",
                table: "file_library_permissions");

            migrationBuilder.DropForeignKey(
                name: "FK_file_library_permissions_users_UserId",
                table: "file_library_permissions");

            migrationBuilder.DropForeignKey(
                name: "FK_message_attachments_messages_MessageId",
                table: "message_attachments");

            migrationBuilder.DropForeignKey(
                name: "FK_message_attachments_users_UploadedByUserId",
                table: "message_attachments");

            migrationBuilder.DropForeignKey(
                name: "FK_messages_cases_RelatedCaseId",
                table: "messages");

            migrationBuilder.DropForeignKey(
                name: "FK_messages_messages_ParentMessageId",
                table: "messages");

            migrationBuilder.DropForeignKey(
                name: "FK_messages_reports_RelatedReportId",
                table: "messages");

            migrationBuilder.DropForeignKey(
                name: "FK_messages_supervised_entities_RelatedEntityId",
                table: "messages");

            migrationBuilder.DropForeignKey(
                name: "FK_messages_users_RecipientId",
                table: "messages");

            migrationBuilder.DropForeignKey(
                name: "FK_messages_users_SenderId",
                table: "messages");

            migrationBuilder.DropForeignKey(
                name: "FK_password_histories_users_UserId",
                table: "password_histories");

            migrationBuilder.DropForeignKey(
                name: "FK_password_policies_users_UpdatedByUserId",
                table: "password_policies");

            migrationBuilder.DropForeignKey(
                name: "FK_refresh_tokens_users_UserId",
                table: "refresh_tokens");

            migrationBuilder.DropForeignKey(
                name: "FK_reports_reports_OriginalReportId",
                table: "reports");

            migrationBuilder.DropForeignKey(
                name: "FK_reports_supervised_entities_SupervisedEntityId",
                table: "reports");

            migrationBuilder.DropForeignKey(
                name: "FK_reports_users_SubmittedByUserId",
                table: "reports");

            migrationBuilder.DropForeignKey(
                name: "FK_role_permissions_permissions_PermissionId",
                table: "role_permissions");

            migrationBuilder.DropForeignKey(
                name: "FK_role_permissions_roles_RoleId",
                table: "role_permissions");

            migrationBuilder.DropForeignKey(
                name: "FK_user_roles_roles_RoleId",
                table: "user_roles");

            migrationBuilder.DropForeignKey(
                name: "FK_user_roles_users_UserId",
                table: "user_roles");

            migrationBuilder.DropForeignKey(
                name: "FK_user_roles_users_UserId1",
                table: "user_roles");

            migrationBuilder.DropForeignKey(
                name: "FK_users_supervised_entities_SupervisedEntityId",
                table: "users");

            migrationBuilder.DropPrimaryKey(
                name: "PK_users",
                table: "users");

            migrationBuilder.DropPrimaryKey(
                name: "PK_user_roles",
                table: "user_roles");

            migrationBuilder.DropPrimaryKey(
                name: "PK_roles",
                table: "roles");

            migrationBuilder.DropPrimaryKey(
                name: "PK_role_permissions",
                table: "role_permissions");

            migrationBuilder.DropPrimaryKey(
                name: "PK_reports",
                table: "reports");

            migrationBuilder.DropPrimaryKey(
                name: "PK_refresh_tokens",
                table: "refresh_tokens");

            migrationBuilder.DropPrimaryKey(
                name: "PK_permissions",
                table: "permissions");

            migrationBuilder.DropPrimaryKey(
                name: "PK_password_policies",
                table: "password_policies");

            migrationBuilder.DropPrimaryKey(
                name: "PK_password_histories",
                table: "password_histories");

            migrationBuilder.DropPrimaryKey(
                name: "PK_messages",
                table: "messages");

            migrationBuilder.DropPrimaryKey(
                name: "PK_message_attachments",
                table: "message_attachments");

            migrationBuilder.DropPrimaryKey(
                name: "PK_file_library_permissions",
                table: "file_library_permissions");

            migrationBuilder.DropPrimaryKey(
                name: "PK_file_libraries",
                table: "file_libraries");

            migrationBuilder.DropPrimaryKey(
                name: "PK_faq_ratings",
                table: "faq_ratings");

            migrationBuilder.DropPrimaryKey(
                name: "PK_faq_questions",
                table: "faq_questions");

            migrationBuilder.DropPrimaryKey(
                name: "PK_contacts",
                table: "contacts");

            migrationBuilder.DropPrimaryKey(
                name: "PK_contact_groups",
                table: "contact_groups");

            migrationBuilder.DropPrimaryKey(
                name: "PK_contact_group_members",
                table: "contact_group_members");

            migrationBuilder.DropPrimaryKey(
                name: "PK_cases",
                table: "cases");

            migrationBuilder.DropPrimaryKey(
                name: "PK_case_histories",
                table: "case_histories");

            migrationBuilder.DropPrimaryKey(
                name: "PK_case_documents",
                table: "case_documents");

            migrationBuilder.DropPrimaryKey(
                name: "PK_audit_logs",
                table: "audit_logs");

            migrationBuilder.DropPrimaryKey(
                name: "PK_announcements",
                table: "announcements");

            migrationBuilder.DropPrimaryKey(
                name: "PK_announcement_recipients",
                table: "announcement_recipients");

            migrationBuilder.DropPrimaryKey(
                name: "PK_announcement_reads",
                table: "announcement_reads");

            migrationBuilder.DropPrimaryKey(
                name: "PK_announcement_histories",
                table: "announcement_histories");

            migrationBuilder.DropPrimaryKey(
                name: "PK_announcement_attachments",
                table: "announcement_attachments");

            migrationBuilder.DropPrimaryKey(
                name: "PK_supervised_entities",
                table: "supervised_entities");

            migrationBuilder.RenameTable(
                name: "supervised_entities",
                newName: "entities");

            migrationBuilder.RenameColumn(
                name: "Phone",
                table: "users",
                newName: "phone");

            migrationBuilder.RenameColumn(
                name: "Email",
                table: "users",
                newName: "email");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "users",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "UpdatedAt",
                table: "users",
                newName: "updated_at");

            migrationBuilder.RenameColumn(
                name: "SupervisedEntityId",
                table: "users",
                newName: "supervised_entity_id");

            migrationBuilder.RenameColumn(
                name: "RequirePasswordChange",
                table: "users",
                newName: "require_password_change");

            migrationBuilder.RenameColumn(
                name: "PasswordHash",
                table: "users",
                newName: "password_hash");

            migrationBuilder.RenameColumn(
                name: "PESEL",
                table: "users",
                newName: "pesel_masked");

            migrationBuilder.RenameColumn(
                name: "LockedUntil",
                table: "users",
                newName: "locked_until");

            migrationBuilder.RenameColumn(
                name: "LastPasswordChangeAt",
                table: "users",
                newName: "password_changed_at");

            migrationBuilder.RenameColumn(
                name: "LastName",
                table: "users",
                newName: "last_name");

            migrationBuilder.RenameColumn(
                name: "LastLoginAt",
                table: "users",
                newName: "last_login_at");

            migrationBuilder.RenameColumn(
                name: "IsActive",
                table: "users",
                newName: "is_active");

            migrationBuilder.RenameColumn(
                name: "FirstName",
                table: "users",
                newName: "first_name");

            migrationBuilder.RenameColumn(
                name: "FailedLoginAttempts",
                table: "users",
                newName: "failed_login_attempts");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                table: "users",
                newName: "created_at");

            migrationBuilder.RenameIndex(
                name: "IX_users_SupervisedEntityId",
                table: "users",
                newName: "i_x_users_supervised_entity_id");

            migrationBuilder.RenameIndex(
                name: "IX_users_Email",
                table: "users",
                newName: "i_x_users_email");

            migrationBuilder.RenameColumn(
                name: "UserId1",
                table: "user_roles",
                newName: "user_id1");

            migrationBuilder.RenameColumn(
                name: "AssignedAt",
                table: "user_roles",
                newName: "assigned_at");

            migrationBuilder.RenameColumn(
                name: "RoleId",
                table: "user_roles",
                newName: "role_id");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "user_roles",
                newName: "user_id");

            migrationBuilder.RenameIndex(
                name: "IX_user_roles_UserId1",
                table: "user_roles",
                newName: "i_x_user_roles_user_id1");

            migrationBuilder.RenameIndex(
                name: "IX_user_roles_RoleId",
                table: "user_roles",
                newName: "i_x_user_roles_role_id");

            migrationBuilder.RenameColumn(
                name: "Name",
                table: "roles",
                newName: "name");

            migrationBuilder.RenameColumn(
                name: "Description",
                table: "roles",
                newName: "description");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "roles",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "UpdatedAt",
                table: "roles",
                newName: "updated_at");

            migrationBuilder.RenameColumn(
                name: "IsSystemRole",
                table: "roles",
                newName: "is_system_role");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                table: "roles",
                newName: "created_at");

            migrationBuilder.RenameIndex(
                name: "IX_roles_Name",
                table: "roles",
                newName: "i_x_roles_name");

            migrationBuilder.RenameColumn(
                name: "PermissionId",
                table: "role_permissions",
                newName: "permission_id");

            migrationBuilder.RenameColumn(
                name: "RoleId",
                table: "role_permissions",
                newName: "role_id");

            migrationBuilder.RenameIndex(
                name: "IX_role_permissions_PermissionId",
                table: "role_permissions",
                newName: "i_x_role_permissions_permission_id");

            migrationBuilder.RenameColumn(
                name: "Status",
                table: "reports",
                newName: "status");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "reports",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "ValidationResultPath",
                table: "reports",
                newName: "validation_result_path");

            migrationBuilder.RenameColumn(
                name: "ValidatedAt",
                table: "reports",
                newName: "validated_at");

            migrationBuilder.RenameColumn(
                name: "SupervisedEntityId",
                table: "reports",
                newName: "supervised_entity_id");

            migrationBuilder.RenameColumn(
                name: "SubmittedByUserId",
                table: "reports",
                newName: "submitted_by_user_id");

            migrationBuilder.RenameColumn(
                name: "SubmittedAt",
                table: "reports",
                newName: "submitted_at");

            migrationBuilder.RenameColumn(
                name: "ReportingPeriod",
                table: "reports",
                newName: "reporting_period");

            migrationBuilder.RenameColumn(
                name: "ReportType",
                table: "reports",
                newName: "report_type");

            migrationBuilder.RenameColumn(
                name: "ReportNumber",
                table: "reports",
                newName: "report_number");

            migrationBuilder.RenameColumn(
                name: "OriginalReportId",
                table: "reports",
                newName: "original_report_id");

            migrationBuilder.RenameColumn(
                name: "IsCorrection",
                table: "reports",
                newName: "is_correction");

            migrationBuilder.RenameColumn(
                name: "FilePath",
                table: "reports",
                newName: "file_path");

            migrationBuilder.RenameColumn(
                name: "FileName",
                table: "reports",
                newName: "file_name");

            migrationBuilder.RenameColumn(
                name: "ErrorDescription",
                table: "reports",
                newName: "error_description");

            migrationBuilder.RenameIndex(
                name: "IX_reports_SupervisedEntityId",
                table: "reports",
                newName: "i_x_reports_supervised_entity_id");

            migrationBuilder.RenameIndex(
                name: "IX_reports_SubmittedByUserId",
                table: "reports",
                newName: "i_x_reports_submitted_by_user_id");

            migrationBuilder.RenameIndex(
                name: "IX_reports_ReportNumber",
                table: "reports",
                newName: "i_x_reports_report_number");

            migrationBuilder.RenameIndex(
                name: "IX_reports_OriginalReportId",
                table: "reports",
                newName: "i_x_reports_original_report_id");

            migrationBuilder.RenameColumn(
                name: "Token",
                table: "refresh_tokens",
                newName: "token");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "refresh_tokens",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "refresh_tokens",
                newName: "user_id");

            migrationBuilder.RenameColumn(
                name: "RevokedByIp",
                table: "refresh_tokens",
                newName: "revoked_by_ip");

            migrationBuilder.RenameColumn(
                name: "RevokedAt",
                table: "refresh_tokens",
                newName: "revoked_at");

            migrationBuilder.RenameColumn(
                name: "RevocationReason",
                table: "refresh_tokens",
                newName: "revocation_reason");

            migrationBuilder.RenameColumn(
                name: "ReplacedByToken",
                table: "refresh_tokens",
                newName: "replaced_by_token");

            migrationBuilder.RenameColumn(
                name: "ExpiresAt",
                table: "refresh_tokens",
                newName: "expires_at");

            migrationBuilder.RenameColumn(
                name: "CreatedByIp",
                table: "refresh_tokens",
                newName: "created_by_ip");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                table: "refresh_tokens",
                newName: "created_at");

            migrationBuilder.RenameIndex(
                name: "IX_refresh_tokens_UserId_ExpiresAt",
                table: "refresh_tokens",
                newName: "i_x_refresh_tokens_user_id_expires_at");

            migrationBuilder.RenameIndex(
                name: "IX_refresh_tokens_Token",
                table: "refresh_tokens",
                newName: "i_x_refresh_tokens_token");

            migrationBuilder.RenameColumn(
                name: "Resource",
                table: "permissions",
                newName: "resource");

            migrationBuilder.RenameColumn(
                name: "Name",
                table: "permissions",
                newName: "name");

            migrationBuilder.RenameColumn(
                name: "Description",
                table: "permissions",
                newName: "description");

            migrationBuilder.RenameColumn(
                name: "Action",
                table: "permissions",
                newName: "action");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "permissions",
                newName: "id");

            migrationBuilder.RenameIndex(
                name: "IX_permissions_Resource_Action",
                table: "permissions",
                newName: "i_x_permissions_resource_action");

            migrationBuilder.RenameIndex(
                name: "IX_permissions_Name",
                table: "permissions",
                newName: "i_x_permissions_name");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "password_policies",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "UpdatedByUserId",
                table: "password_policies",
                newName: "updated_by_user_id");

            migrationBuilder.RenameColumn(
                name: "UpdatedAt",
                table: "password_policies",
                newName: "updated_at");

            migrationBuilder.RenameColumn(
                name: "RequireUppercase",
                table: "password_policies",
                newName: "require_uppercase");

            migrationBuilder.RenameColumn(
                name: "RequireSpecialChar",
                table: "password_policies",
                newName: "require_special_char");

            migrationBuilder.RenameColumn(
                name: "RequireLowercase",
                table: "password_policies",
                newName: "require_lowercase");

            migrationBuilder.RenameColumn(
                name: "RequireDigit",
                table: "password_policies",
                newName: "require_digit");

            migrationBuilder.RenameColumn(
                name: "MinLength",
                table: "password_policies",
                newName: "min_length");

            migrationBuilder.RenameColumn(
                name: "MaxFailedAttempts",
                table: "password_policies",
                newName: "max_failed_attempts");

            migrationBuilder.RenameColumn(
                name: "LockoutDurationMinutes",
                table: "password_policies",
                newName: "lockout_duration_minutes");

            migrationBuilder.RenameColumn(
                name: "HistoryCount",
                table: "password_policies",
                newName: "history_count");

            migrationBuilder.RenameColumn(
                name: "ExpirationDays",
                table: "password_policies",
                newName: "expiration_days");

            migrationBuilder.RenameIndex(
                name: "IX_password_policies_UpdatedByUserId",
                table: "password_policies",
                newName: "i_x_password_policies_updated_by_user_id");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "password_histories",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "password_histories",
                newName: "user_id");

            migrationBuilder.RenameColumn(
                name: "PasswordHash",
                table: "password_histories",
                newName: "password_hash");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                table: "password_histories",
                newName: "created_at");

            migrationBuilder.RenameIndex(
                name: "IX_password_histories_UserId_CreatedAt",
                table: "password_histories",
                newName: "i_x_password_histories_user_id_created_at");

            migrationBuilder.RenameColumn(
                name: "Subject",
                table: "messages",
                newName: "subject");

            migrationBuilder.RenameColumn(
                name: "Status",
                table: "messages",
                newName: "status");

            migrationBuilder.RenameColumn(
                name: "Folder",
                table: "messages",
                newName: "folder");

            migrationBuilder.RenameColumn(
                name: "Body",
                table: "messages",
                newName: "body");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "messages",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "ThreadId",
                table: "messages",
                newName: "thread_id");

            migrationBuilder.RenameColumn(
                name: "SentAt",
                table: "messages",
                newName: "sent_at");

            migrationBuilder.RenameColumn(
                name: "SenderId",
                table: "messages",
                newName: "sender_id");

            migrationBuilder.RenameColumn(
                name: "RelatedReportId",
                table: "messages",
                newName: "related_report_id");

            migrationBuilder.RenameColumn(
                name: "RelatedEntityId",
                table: "messages",
                newName: "related_entity_id");

            migrationBuilder.RenameColumn(
                name: "RelatedCaseId",
                table: "messages",
                newName: "related_case_id");

            migrationBuilder.RenameColumn(
                name: "RecipientId",
                table: "messages",
                newName: "recipient_id");

            migrationBuilder.RenameColumn(
                name: "ReadAt",
                table: "messages",
                newName: "read_at");

            migrationBuilder.RenameColumn(
                name: "ParentMessageId",
                table: "messages",
                newName: "parent_message_id");

            migrationBuilder.RenameColumn(
                name: "IsRead",
                table: "messages",
                newName: "is_read");

            migrationBuilder.RenameColumn(
                name: "IsCancelled",
                table: "messages",
                newName: "is_cancelled");

            migrationBuilder.RenameColumn(
                name: "CancelledAt",
                table: "messages",
                newName: "cancelled_at");

            migrationBuilder.RenameIndex(
                name: "IX_messages_ThreadId",
                table: "messages",
                newName: "i_x_messages_thread_id");

            migrationBuilder.RenameIndex(
                name: "IX_messages_SenderId_SentAt",
                table: "messages",
                newName: "i_x_messages_sender_id_sent_at");

            migrationBuilder.RenameIndex(
                name: "IX_messages_RelatedReportId",
                table: "messages",
                newName: "i_x_messages_related_report_id");

            migrationBuilder.RenameIndex(
                name: "IX_messages_RelatedEntityId",
                table: "messages",
                newName: "i_x_messages_related_entity_id");

            migrationBuilder.RenameIndex(
                name: "IX_messages_RelatedCaseId",
                table: "messages",
                newName: "i_x_messages_related_case_id");

            migrationBuilder.RenameIndex(
                name: "IX_messages_RecipientId_IsRead",
                table: "messages",
                newName: "i_x_messages_recipient_id_is_read");

            migrationBuilder.RenameIndex(
                name: "IX_messages_ParentMessageId",
                table: "messages",
                newName: "i_x_messages_parent_message_id");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "message_attachments",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "UploadedByUserId",
                table: "message_attachments",
                newName: "uploaded_by_user_id");

            migrationBuilder.RenameColumn(
                name: "UploadedAt",
                table: "message_attachments",
                newName: "uploaded_at");

            migrationBuilder.RenameColumn(
                name: "MessageId",
                table: "message_attachments",
                newName: "message_id");

            migrationBuilder.RenameColumn(
                name: "FileSize",
                table: "message_attachments",
                newName: "file_size");

            migrationBuilder.RenameColumn(
                name: "FilePath",
                table: "message_attachments",
                newName: "file_path");

            migrationBuilder.RenameColumn(
                name: "FileName",
                table: "message_attachments",
                newName: "file_name");

            migrationBuilder.RenameColumn(
                name: "ContentType",
                table: "message_attachments",
                newName: "content_type");

            migrationBuilder.RenameIndex(
                name: "IX_message_attachments_UploadedByUserId",
                table: "message_attachments",
                newName: "i_x_message_attachments_uploaded_by_user_id");

            migrationBuilder.RenameIndex(
                name: "IX_message_attachments_MessageId",
                table: "message_attachments",
                newName: "i_x_message_attachments_message_id");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "file_library_permissions",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "file_library_permissions",
                newName: "user_id");

            migrationBuilder.RenameColumn(
                name: "SupervisedEntityId",
                table: "file_library_permissions",
                newName: "supervised_entity_id");

            migrationBuilder.RenameColumn(
                name: "RoleName",
                table: "file_library_permissions",
                newName: "role_name");

            migrationBuilder.RenameColumn(
                name: "PodmiotType",
                table: "file_library_permissions",
                newName: "podmiot_type");

            migrationBuilder.RenameColumn(
                name: "PermissionType",
                table: "file_library_permissions",
                newName: "permission_type");

            migrationBuilder.RenameColumn(
                name: "FileLibraryId",
                table: "file_library_permissions",
                newName: "file_library_id");

            migrationBuilder.RenameColumn(
                name: "CanWrite",
                table: "file_library_permissions",
                newName: "can_write");

            migrationBuilder.RenameColumn(
                name: "CanRead",
                table: "file_library_permissions",
                newName: "can_read");

            migrationBuilder.RenameColumn(
                name: "CanDelete",
                table: "file_library_permissions",
                newName: "can_delete");

            migrationBuilder.RenameIndex(
                name: "IX_file_library_permissions_UserId",
                table: "file_library_permissions",
                newName: "i_x_file_library_permissions_user_id");

            migrationBuilder.RenameIndex(
                name: "IX_file_library_permissions_SupervisedEntityId",
                table: "file_library_permissions",
                newName: "i_x_file_library_permissions_supervised_entity_id");

            migrationBuilder.RenameIndex(
                name: "IX_file_library_permissions_FileLibraryId_PermissionType",
                table: "file_library_permissions",
                newName: "i_x_file_library_permissions_file_library_id_permission_type");

            migrationBuilder.RenameColumn(
                name: "Version",
                table: "file_libraries",
                newName: "version");

            migrationBuilder.RenameColumn(
                name: "Tags",
                table: "file_libraries",
                newName: "tags");

            migrationBuilder.RenameColumn(
                name: "Name",
                table: "file_libraries",
                newName: "name");

            migrationBuilder.RenameColumn(
                name: "Description",
                table: "file_libraries",
                newName: "description");

            migrationBuilder.RenameColumn(
                name: "Category",
                table: "file_libraries",
                newName: "category");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "file_libraries",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "UploadedByUserId",
                table: "file_libraries",
                newName: "uploaded_by_user_id");

            migrationBuilder.RenameColumn(
                name: "UploadedAt",
                table: "file_libraries",
                newName: "uploaded_at");

            migrationBuilder.RenameColumn(
                name: "ParentFileId",
                table: "file_libraries",
                newName: "parent_file_id");

            migrationBuilder.RenameColumn(
                name: "IsPublic",
                table: "file_libraries",
                newName: "is_public");

            migrationBuilder.RenameColumn(
                name: "IsCurrentVersion",
                table: "file_libraries",
                newName: "is_current_version");

            migrationBuilder.RenameColumn(
                name: "FileSize",
                table: "file_libraries",
                newName: "file_size");

            migrationBuilder.RenameColumn(
                name: "FilePath",
                table: "file_libraries",
                newName: "file_path");

            migrationBuilder.RenameColumn(
                name: "FileName",
                table: "file_libraries",
                newName: "file_name");

            migrationBuilder.RenameColumn(
                name: "DownloadCount",
                table: "file_libraries",
                newName: "download_count");

            migrationBuilder.RenameColumn(
                name: "ContentType",
                table: "file_libraries",
                newName: "content_type");

            migrationBuilder.RenameIndex(
                name: "IX_file_libraries_UploadedByUserId",
                table: "file_libraries",
                newName: "i_x_file_libraries_uploaded_by_user_id");

            migrationBuilder.RenameIndex(
                name: "IX_file_libraries_UploadedAt",
                table: "file_libraries",
                newName: "i_x_file_libraries_uploaded_at");

            migrationBuilder.RenameIndex(
                name: "IX_file_libraries_ParentFileId",
                table: "file_libraries",
                newName: "i_x_file_libraries_parent_file_id");

            migrationBuilder.RenameIndex(
                name: "IX_file_libraries_Category_IsCurrentVersion",
                table: "file_libraries",
                newName: "i_x_file_libraries_category_is_current_version");

            migrationBuilder.RenameColumn(
                name: "Rating",
                table: "faq_ratings",
                newName: "rating");

            migrationBuilder.RenameColumn(
                name: "Comment",
                table: "faq_ratings",
                newName: "comment");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "faq_ratings",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "faq_ratings",
                newName: "user_id");

            migrationBuilder.RenameColumn(
                name: "RatedAt",
                table: "faq_ratings",
                newName: "rated_at");

            migrationBuilder.RenameColumn(
                name: "FaqQuestionId",
                table: "faq_ratings",
                newName: "faq_question_id");

            migrationBuilder.RenameIndex(
                name: "IX_faq_ratings_UserId",
                table: "faq_ratings",
                newName: "i_x_faq_ratings_user_id");

            migrationBuilder.RenameIndex(
                name: "IX_faq_ratings_FaqQuestionId_UserId",
                table: "faq_ratings",
                newName: "i_x_faq_ratings_faq_question_id_user_id");

            migrationBuilder.RenameColumn(
                name: "Title",
                table: "faq_questions",
                newName: "title");

            migrationBuilder.RenameColumn(
                name: "Tags",
                table: "faq_questions",
                newName: "tags");

            migrationBuilder.RenameColumn(
                name: "Status",
                table: "faq_questions",
                newName: "status");

            migrationBuilder.RenameColumn(
                name: "Content",
                table: "faq_questions",
                newName: "content");

            migrationBuilder.RenameColumn(
                name: "Category",
                table: "faq_questions",
                newName: "category");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "faq_questions",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "ViewCount",
                table: "faq_questions",
                newName: "view_count");

            migrationBuilder.RenameColumn(
                name: "SubmittedByUserId",
                table: "faq_questions",
                newName: "submitted_by_user_id");

            migrationBuilder.RenameColumn(
                name: "SubmittedAt",
                table: "faq_questions",
                newName: "submitted_at");

            migrationBuilder.RenameColumn(
                name: "RatingCount",
                table: "faq_questions",
                newName: "rating_count");

            migrationBuilder.RenameColumn(
                name: "PublishedAt",
                table: "faq_questions",
                newName: "published_at");

            migrationBuilder.RenameColumn(
                name: "AverageRating",
                table: "faq_questions",
                newName: "average_rating");

            migrationBuilder.RenameColumn(
                name: "AnsweredByUserId",
                table: "faq_questions",
                newName: "answered_by_user_id");

            migrationBuilder.RenameColumn(
                name: "AnsweredAt",
                table: "faq_questions",
                newName: "answered_at");

            migrationBuilder.RenameColumn(
                name: "AnswerContent",
                table: "faq_questions",
                newName: "answer_content");

            migrationBuilder.RenameColumn(
                name: "AnonymousName",
                table: "faq_questions",
                newName: "anonymous_name");

            migrationBuilder.RenameColumn(
                name: "AnonymousEmail",
                table: "faq_questions",
                newName: "anonymous_email");

            migrationBuilder.RenameIndex(
                name: "IX_faq_questions_SubmittedByUserId",
                table: "faq_questions",
                newName: "i_x_faq_questions_submitted_by_user_id");

            migrationBuilder.RenameIndex(
                name: "IX_faq_questions_SubmittedAt",
                table: "faq_questions",
                newName: "i_x_faq_questions_submitted_at");

            migrationBuilder.RenameIndex(
                name: "IX_faq_questions_Status_Category",
                table: "faq_questions",
                newName: "i_x_faq_questions_status_category");

            migrationBuilder.RenameIndex(
                name: "IX_faq_questions_PublishedAt",
                table: "faq_questions",
                newName: "i_x_faq_questions_published_at");

            migrationBuilder.RenameIndex(
                name: "IX_faq_questions_AnsweredByUserId",
                table: "faq_questions",
                newName: "i_x_faq_questions_answered_by_user_id");

            migrationBuilder.RenameColumn(
                name: "Position",
                table: "contacts",
                newName: "position");

            migrationBuilder.RenameColumn(
                name: "Phone",
                table: "contacts",
                newName: "phone");

            migrationBuilder.RenameColumn(
                name: "Notes",
                table: "contacts",
                newName: "notes");

            migrationBuilder.RenameColumn(
                name: "Name",
                table: "contacts",
                newName: "name");

            migrationBuilder.RenameColumn(
                name: "Mobile",
                table: "contacts",
                newName: "mobile");

            migrationBuilder.RenameColumn(
                name: "Email",
                table: "contacts",
                newName: "email");

            migrationBuilder.RenameColumn(
                name: "Department",
                table: "contacts",
                newName: "department");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "contacts",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "UpdatedAt",
                table: "contacts",
                newName: "updated_at");

            migrationBuilder.RenameColumn(
                name: "SupervisedEntityId",
                table: "contacts",
                newName: "supervised_entity_id");

            migrationBuilder.RenameColumn(
                name: "IsPrimary",
                table: "contacts",
                newName: "is_primary");

            migrationBuilder.RenameColumn(
                name: "IsActive",
                table: "contacts",
                newName: "is_active");

            migrationBuilder.RenameColumn(
                name: "CreatedByUserId",
                table: "contacts",
                newName: "created_by_user_id");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                table: "contacts",
                newName: "created_at");

            migrationBuilder.RenameIndex(
                name: "IX_contacts_SupervisedEntityId_IsPrimary",
                table: "contacts",
                newName: "i_x_contacts_supervised_entity_id_is_primary");

            migrationBuilder.RenameIndex(
                name: "IX_contacts_Email",
                table: "contacts",
                newName: "i_x_contacts_email");

            migrationBuilder.RenameIndex(
                name: "IX_contacts_CreatedByUserId",
                table: "contacts",
                newName: "i_x_contacts_created_by_user_id");

            migrationBuilder.RenameColumn(
                name: "Name",
                table: "contact_groups",
                newName: "name");

            migrationBuilder.RenameColumn(
                name: "Description",
                table: "contact_groups",
                newName: "description");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "contact_groups",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "CreatedByUserId",
                table: "contact_groups",
                newName: "created_by_user_id");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                table: "contact_groups",
                newName: "created_at");

            migrationBuilder.RenameIndex(
                name: "IX_contact_groups_CreatedByUserId",
                table: "contact_groups",
                newName: "i_x_contact_groups_created_by_user_id");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "contact_group_members",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "ContactId",
                table: "contact_group_members",
                newName: "contact_id");

            migrationBuilder.RenameColumn(
                name: "ContactGroupId",
                table: "contact_group_members",
                newName: "contact_group_id");

            migrationBuilder.RenameColumn(
                name: "AddedAt",
                table: "contact_group_members",
                newName: "added_at");

            migrationBuilder.RenameIndex(
                name: "IX_contact_group_members_ContactId",
                table: "contact_group_members",
                newName: "i_x_contact_group_members_contact_id");

            migrationBuilder.RenameIndex(
                name: "IX_contact_group_members_ContactGroupId_ContactId",
                table: "contact_group_members",
                newName: "i_x_contact_group_members_contact_group_id_contact_id");

            migrationBuilder.RenameColumn(
                name: "Title",
                table: "cases",
                newName: "title");

            migrationBuilder.RenameColumn(
                name: "Status",
                table: "cases",
                newName: "status");

            migrationBuilder.RenameColumn(
                name: "Priority",
                table: "cases",
                newName: "priority");

            migrationBuilder.RenameColumn(
                name: "Description",
                table: "cases",
                newName: "description");

            migrationBuilder.RenameColumn(
                name: "Category",
                table: "cases",
                newName: "category");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "cases",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "UpdatedAt",
                table: "cases",
                newName: "updated_at");

            migrationBuilder.RenameColumn(
                name: "SupervisedEntityId",
                table: "cases",
                newName: "supervised_entity_id");

            migrationBuilder.RenameColumn(
                name: "ResolvedAt",
                table: "cases",
                newName: "resolved_at");

            migrationBuilder.RenameColumn(
                name: "IsCancelled",
                table: "cases",
                newName: "is_cancelled");

            migrationBuilder.RenameColumn(
                name: "HandlerId",
                table: "cases",
                newName: "handler_id");

            migrationBuilder.RenameColumn(
                name: "CreatedByUserId",
                table: "cases",
                newName: "created_by_user_id");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                table: "cases",
                newName: "created_at");

            migrationBuilder.RenameColumn(
                name: "ClosedAt",
                table: "cases",
                newName: "closed_at");

            migrationBuilder.RenameColumn(
                name: "CaseNumber",
                table: "cases",
                newName: "case_number");

            migrationBuilder.RenameColumn(
                name: "CancelledAt",
                table: "cases",
                newName: "cancelled_at");

            migrationBuilder.RenameColumn(
                name: "CancellationReason",
                table: "cases",
                newName: "cancellation_reason");

            migrationBuilder.RenameIndex(
                name: "IX_cases_SupervisedEntityId_Status",
                table: "cases",
                newName: "i_x_cases_supervised_entity_id_status");

            migrationBuilder.RenameIndex(
                name: "IX_cases_HandlerId",
                table: "cases",
                newName: "i_x_cases_handler_id");

            migrationBuilder.RenameIndex(
                name: "IX_cases_CreatedByUserId",
                table: "cases",
                newName: "i_x_cases_created_by_user_id");

            migrationBuilder.RenameIndex(
                name: "IX_cases_CreatedAt",
                table: "cases",
                newName: "i_x_cases_created_at");

            migrationBuilder.RenameIndex(
                name: "IX_cases_CaseNumber",
                table: "cases",
                newName: "i_x_cases_case_number");

            migrationBuilder.RenameColumn(
                name: "Description",
                table: "case_histories",
                newName: "description");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "case_histories",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "OldStatus",
                table: "case_histories",
                newName: "old_status");

            migrationBuilder.RenameColumn(
                name: "NewStatus",
                table: "case_histories",
                newName: "new_status");

            migrationBuilder.RenameColumn(
                name: "ChangedByUserId",
                table: "case_histories",
                newName: "changed_by_user_id");

            migrationBuilder.RenameColumn(
                name: "ChangedAt",
                table: "case_histories",
                newName: "changed_at");

            migrationBuilder.RenameColumn(
                name: "ChangeType",
                table: "case_histories",
                newName: "change_type");

            migrationBuilder.RenameColumn(
                name: "CaseId",
                table: "case_histories",
                newName: "case_id");

            migrationBuilder.RenameIndex(
                name: "IX_case_histories_ChangedByUserId",
                table: "case_histories",
                newName: "i_x_case_histories_changed_by_user_id");

            migrationBuilder.RenameIndex(
                name: "IX_case_histories_CaseId_ChangedAt",
                table: "case_histories",
                newName: "i_x_case_histories_case_id_changed_at");

            migrationBuilder.RenameColumn(
                name: "Description",
                table: "case_documents",
                newName: "description");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "case_documents",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "UploadedByUserId",
                table: "case_documents",
                newName: "uploaded_by_user_id");

            migrationBuilder.RenameColumn(
                name: "UploadedAt",
                table: "case_documents",
                newName: "uploaded_at");

            migrationBuilder.RenameColumn(
                name: "FileSize",
                table: "case_documents",
                newName: "file_size");

            migrationBuilder.RenameColumn(
                name: "FilePath",
                table: "case_documents",
                newName: "file_path");

            migrationBuilder.RenameColumn(
                name: "FileName",
                table: "case_documents",
                newName: "file_name");

            migrationBuilder.RenameColumn(
                name: "DocumentName",
                table: "case_documents",
                newName: "document_name");

            migrationBuilder.RenameColumn(
                name: "ContentType",
                table: "case_documents",
                newName: "content_type");

            migrationBuilder.RenameColumn(
                name: "CaseId",
                table: "case_documents",
                newName: "case_id");

            migrationBuilder.RenameIndex(
                name: "IX_case_documents_UploadedByUserId",
                table: "case_documents",
                newName: "i_x_case_documents_uploaded_by_user_id");

            migrationBuilder.RenameIndex(
                name: "IX_case_documents_CaseId",
                table: "case_documents",
                newName: "i_x_case_documents_case_id");

            migrationBuilder.RenameColumn(
                name: "Timestamp",
                table: "audit_logs",
                newName: "timestamp");

            migrationBuilder.RenameColumn(
                name: "Resource",
                table: "audit_logs",
                newName: "resource");

            migrationBuilder.RenameColumn(
                name: "Details",
                table: "audit_logs",
                newName: "details");

            migrationBuilder.RenameColumn(
                name: "Action",
                table: "audit_logs",
                newName: "action");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "audit_logs",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "audit_logs",
                newName: "user_id");

            migrationBuilder.RenameColumn(
                name: "ResourceId",
                table: "audit_logs",
                newName: "resource_id");

            migrationBuilder.RenameColumn(
                name: "IpAddress",
                table: "audit_logs",
                newName: "ip_address");

            migrationBuilder.RenameIndex(
                name: "IX_audit_logs_UserId_Timestamp",
                table: "audit_logs",
                newName: "i_x_audit_logs_user_id_timestamp");

            migrationBuilder.RenameIndex(
                name: "IX_audit_logs_Timestamp",
                table: "audit_logs",
                newName: "i_x_audit_logs_timestamp");

            migrationBuilder.RenameIndex(
                name: "IX_audit_logs_Resource_Action",
                table: "audit_logs",
                newName: "i_x_audit_logs_resource_action");

            migrationBuilder.RenameColumn(
                name: "Title",
                table: "announcements",
                newName: "title");

            migrationBuilder.RenameColumn(
                name: "Priority",
                table: "announcements",
                newName: "priority");

            migrationBuilder.RenameColumn(
                name: "Content",
                table: "announcements",
                newName: "content");

            migrationBuilder.RenameColumn(
                name: "Category",
                table: "announcements",
                newName: "category");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "announcements",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "UpdatedAt",
                table: "announcements",
                newName: "updated_at");

            migrationBuilder.RenameColumn(
                name: "PublishedAt",
                table: "announcements",
                newName: "published_at");

            migrationBuilder.RenameColumn(
                name: "IsPublished",
                table: "announcements",
                newName: "is_published");

            migrationBuilder.RenameColumn(
                name: "ExpiresAt",
                table: "announcements",
                newName: "expires_at");

            migrationBuilder.RenameColumn(
                name: "CreatedByUserId",
                table: "announcements",
                newName: "created_by_user_id");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                table: "announcements",
                newName: "created_at");

            migrationBuilder.RenameIndex(
                name: "IX_announcements_IsPublished_PublishedAt",
                table: "announcements",
                newName: "i_x_announcements_is_published_published_at");

            migrationBuilder.RenameIndex(
                name: "IX_announcements_ExpiresAt",
                table: "announcements",
                newName: "i_x_announcements_expires_at");

            migrationBuilder.RenameIndex(
                name: "IX_announcements_CreatedByUserId",
                table: "announcements",
                newName: "i_x_announcements_created_by_user_id");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "announcement_recipients",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "announcement_recipients",
                newName: "user_id");

            migrationBuilder.RenameColumn(
                name: "SupervisedEntityId",
                table: "announcement_recipients",
                newName: "supervised_entity_id");

            migrationBuilder.RenameColumn(
                name: "RecipientType",
                table: "announcement_recipients",
                newName: "recipient_type");

            migrationBuilder.RenameColumn(
                name: "PodmiotType",
                table: "announcement_recipients",
                newName: "podmiot_type");

            migrationBuilder.RenameColumn(
                name: "AnnouncementId",
                table: "announcement_recipients",
                newName: "announcement_id");

            migrationBuilder.RenameIndex(
                name: "IX_announcement_recipients_UserId",
                table: "announcement_recipients",
                newName: "i_x_announcement_recipients_user_id");

            migrationBuilder.RenameIndex(
                name: "IX_announcement_recipients_SupervisedEntityId",
                table: "announcement_recipients",
                newName: "i_x_announcement_recipients_supervised_entity_id");

            migrationBuilder.RenameIndex(
                name: "IX_announcement_recipients_AnnouncementId_RecipientType",
                table: "announcement_recipients",
                newName: "i_x_announcement_recipients_announcement_id_recipient_type");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "announcement_reads",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "announcement_reads",
                newName: "user_id");

            migrationBuilder.RenameColumn(
                name: "ReadAt",
                table: "announcement_reads",
                newName: "read_at");

            migrationBuilder.RenameColumn(
                name: "IpAddress",
                table: "announcement_reads",
                newName: "ip_address");

            migrationBuilder.RenameColumn(
                name: "AnnouncementId",
                table: "announcement_reads",
                newName: "announcement_id");

            migrationBuilder.RenameIndex(
                name: "IX_announcement_reads_UserId",
                table: "announcement_reads",
                newName: "i_x_announcement_reads_user_id");

            migrationBuilder.RenameIndex(
                name: "IX_announcement_reads_ReadAt",
                table: "announcement_reads",
                newName: "i_x_announcement_reads_read_at");

            migrationBuilder.RenameIndex(
                name: "IX_announcement_reads_AnnouncementId_UserId",
                table: "announcement_reads",
                newName: "i_x_announcement_reads_announcement_id_user_id");

            migrationBuilder.RenameColumn(
                name: "Description",
                table: "announcement_histories",
                newName: "description");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "announcement_histories",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "ChangedByUserId",
                table: "announcement_histories",
                newName: "changed_by_user_id");

            migrationBuilder.RenameColumn(
                name: "ChangedAt",
                table: "announcement_histories",
                newName: "changed_at");

            migrationBuilder.RenameColumn(
                name: "ChangeType",
                table: "announcement_histories",
                newName: "change_type");

            migrationBuilder.RenameColumn(
                name: "AnnouncementId",
                table: "announcement_histories",
                newName: "announcement_id");

            migrationBuilder.RenameIndex(
                name: "IX_announcement_histories_ChangedByUserId",
                table: "announcement_histories",
                newName: "i_x_announcement_histories_changed_by_user_id");

            migrationBuilder.RenameIndex(
                name: "IX_announcement_histories_AnnouncementId_ChangedAt",
                table: "announcement_histories",
                newName: "i_x_announcement_histories_announcement_id_changed_at");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "announcement_attachments",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "UploadedAt",
                table: "announcement_attachments",
                newName: "uploaded_at");

            migrationBuilder.RenameColumn(
                name: "FileSize",
                table: "announcement_attachments",
                newName: "file_size");

            migrationBuilder.RenameColumn(
                name: "FilePath",
                table: "announcement_attachments",
                newName: "file_path");

            migrationBuilder.RenameColumn(
                name: "FileName",
                table: "announcement_attachments",
                newName: "file_name");

            migrationBuilder.RenameColumn(
                name: "ContentType",
                table: "announcement_attachments",
                newName: "content_type");

            migrationBuilder.RenameColumn(
                name: "AnnouncementId",
                table: "announcement_attachments",
                newName: "announcement_id");

            migrationBuilder.RenameIndex(
                name: "IX_announcement_attachments_AnnouncementId",
                table: "announcement_attachments",
                newName: "i_x_announcement_attachments_announcement_id");

            migrationBuilder.RenameColumn(
                name: "Website",
                table: "entities",
                newName: "website");

            migrationBuilder.RenameColumn(
                name: "Street",
                table: "entities",
                newName: "street");

            migrationBuilder.RenameColumn(
                name: "Phone",
                table: "entities",
                newName: "phone");

            migrationBuilder.RenameColumn(
                name: "Email",
                table: "entities",
                newName: "email");

            migrationBuilder.RenameColumn(
                name: "Country",
                table: "entities",
                newName: "country");

            migrationBuilder.RenameColumn(
                name: "City",
                table: "entities",
                newName: "city");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "entities",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "UpdatedAt",
                table: "entities",
                newName: "updated_at");

            migrationBuilder.RenameColumn(
                name: "UKNFCode",
                table: "entities",
                newName: "uknf_code");

            migrationBuilder.RenameColumn(
                name: "Subsector",
                table: "entities",
                newName: "entity_subsector");

            migrationBuilder.RenameColumn(
                name: "Status",
                table: "entities",
                newName: "entity_status");

            migrationBuilder.RenameColumn(
                name: "Sector",
                table: "entities",
                newName: "entity_sector");

            migrationBuilder.RenameColumn(
                name: "RegistryNumber",
                table: "entities",
                newName: "uknf_registry_number");

            migrationBuilder.RenameColumn(
                name: "REGON",
                table: "entities",
                newName: "r_e_g_o_n");

            migrationBuilder.RenameColumn(
                name: "PostalCode",
                table: "entities",
                newName: "postal_code");

            migrationBuilder.RenameColumn(
                name: "Name",
                table: "entities",
                newName: "entity_name");

            migrationBuilder.RenameColumn(
                name: "NIP",
                table: "entities",
                newName: "n_i_p");

            migrationBuilder.RenameColumn(
                name: "LEI",
                table: "entities",
                newName: "l_e_i");

            migrationBuilder.RenameColumn(
                name: "KRS",
                table: "entities",
                newName: "k_r_s");

            migrationBuilder.RenameColumn(
                name: "IsCrossBorder",
                table: "entities",
                newName: "is_cross_border");

            migrationBuilder.RenameColumn(
                name: "IsActive",
                table: "entities",
                newName: "is_active");

            migrationBuilder.RenameColumn(
                name: "EntityType",
                table: "entities",
                newName: "entity_type");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                table: "entities",
                newName: "created_at");

            migrationBuilder.RenameColumn(
                name: "Category",
                table: "entities",
                newName: "entity_category");

            migrationBuilder.RenameColumn(
                name: "BuildingNumber",
                table: "entities",
                newName: "building_number");

            migrationBuilder.RenameColumn(
                name: "ApartmentNumber",
                table: "entities",
                newName: "apartment_number");

            migrationBuilder.RenameIndex(
                name: "IX_supervised_entities_UKNFCode",
                table: "entities",
                newName: "i_x_entities_uknf_code");

            migrationBuilder.AlterColumn<string>(
                name: "phone",
                table: "users",
                type: "character varying(250)",
                maxLength: 250,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "password_hash",
                table: "users",
                type: "character varying(500)",
                maxLength: 500,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "pesel_masked",
                table: "users",
                type: "character varying(250)",
                maxLength: 250,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "status",
                table: "reports",
                type: "text",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<string>(
                name: "status",
                table: "messages",
                type: "text",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<string>(
                name: "folder",
                table: "messages",
                type: "text",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<string>(
                name: "status",
                table: "faq_questions",
                type: "text",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<string>(
                name: "status",
                table: "cases",
                type: "text",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<string>(
                name: "priority",
                table: "announcements",
                type: "text",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<string>(
                name: "entity_subsector",
                table: "entities",
                type: "character varying(500)",
                maxLength: 500,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "entity_status",
                table: "entities",
                type: "character varying(250)",
                maxLength: 250,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "entity_sector",
                table: "entities",
                type: "character varying(500)",
                maxLength: 500,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "uknf_registry_number",
                table: "entities",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "n_i_p",
                table: "entities",
                type: "character varying(10)",
                maxLength: 10,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "l_e_i",
                table: "entities",
                type: "character varying(20)",
                maxLength: 20,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "k_r_s",
                table: "entities",
                type: "character varying(10)",
                maxLength: 10,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "entity_type",
                table: "entities",
                type: "character varying(250)",
                maxLength: 250,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "entity_category",
                table: "entities",
                type: "character varying(500)",
                maxLength: 500,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "p_k_users",
                table: "users",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "p_k_user_roles",
                table: "user_roles",
                columns: new[] { "user_id", "role_id" });

            migrationBuilder.AddPrimaryKey(
                name: "p_k_roles",
                table: "roles",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "p_k_role_permissions",
                table: "role_permissions",
                columns: new[] { "role_id", "permission_id" });

            migrationBuilder.AddPrimaryKey(
                name: "p_k_reports",
                table: "reports",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "p_k_refresh_tokens",
                table: "refresh_tokens",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "p_k_permissions",
                table: "permissions",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "p_k_password_policies",
                table: "password_policies",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "p_k_password_histories",
                table: "password_histories",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "p_k_messages",
                table: "messages",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "p_k_message_attachments",
                table: "message_attachments",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "p_k_file_library_permissions",
                table: "file_library_permissions",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "p_k_file_libraries",
                table: "file_libraries",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "p_k_faq_ratings",
                table: "faq_ratings",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "p_k_faq_questions",
                table: "faq_questions",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "p_k_contacts",
                table: "contacts",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "p_k_contact_groups",
                table: "contact_groups",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "p_k_contact_group_members",
                table: "contact_group_members",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "p_k_cases",
                table: "cases",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "p_k_case_histories",
                table: "case_histories",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "p_k_case_documents",
                table: "case_documents",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "p_k_audit_logs",
                table: "audit_logs",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "p_k_announcements",
                table: "announcements",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "p_k_announcement_recipients",
                table: "announcement_recipients",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "p_k_announcement_reads",
                table: "announcement_reads",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "p_k_announcement_histories",
                table: "announcement_histories",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "p_k_announcement_attachments",
                table: "announcement_attachments",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "p_k_entities",
                table: "entities",
                column: "id");

            migrationBuilder.AddForeignKey(
                name: "f_k_announcement_attachments_announcements_announcement_id",
                table: "announcement_attachments",
                column: "announcement_id",
                principalTable: "announcements",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "f_k_announcement_histories_announcements_announcement_id",
                table: "announcement_histories",
                column: "announcement_id",
                principalTable: "announcements",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "f_k_announcement_histories_users_changed_by_user_id",
                table: "announcement_histories",
                column: "changed_by_user_id",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "f_k_announcement_reads_announcements_announcement_id",
                table: "announcement_reads",
                column: "announcement_id",
                principalTable: "announcements",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "f_k_announcement_reads_users_user_id",
                table: "announcement_reads",
                column: "user_id",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "f_k_announcement_recipients_announcements_announcement_id",
                table: "announcement_recipients",
                column: "announcement_id",
                principalTable: "announcements",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "f_k_announcement_recipients_entities_supervised_entity_id",
                table: "announcement_recipients",
                column: "supervised_entity_id",
                principalTable: "entities",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "f_k_announcement_recipients_users_user_id",
                table: "announcement_recipients",
                column: "user_id",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "f_k_announcements_users_created_by_user_id",
                table: "announcements",
                column: "created_by_user_id",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "f_k_audit_logs_users_user_id",
                table: "audit_logs",
                column: "user_id",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "f_k_case_documents_cases_case_id",
                table: "case_documents",
                column: "case_id",
                principalTable: "cases",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "f_k_case_documents_users_uploaded_by_user_id",
                table: "case_documents",
                column: "uploaded_by_user_id",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "f_k_case_histories_cases_case_id",
                table: "case_histories",
                column: "case_id",
                principalTable: "cases",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "f_k_case_histories_users_changed_by_user_id",
                table: "case_histories",
                column: "changed_by_user_id",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "f_k_cases_entities_supervised_entity_id",
                table: "cases",
                column: "supervised_entity_id",
                principalTable: "entities",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "f_k_cases_users_created_by_user_id",
                table: "cases",
                column: "created_by_user_id",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "f_k_cases_users_handler_id",
                table: "cases",
                column: "handler_id",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "f_k_contact_group_members_contact_groups_contact_group_id",
                table: "contact_group_members",
                column: "contact_group_id",
                principalTable: "contact_groups",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "f_k_contact_group_members_contacts_contact_id",
                table: "contact_group_members",
                column: "contact_id",
                principalTable: "contacts",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "f_k_contact_groups_users_created_by_user_id",
                table: "contact_groups",
                column: "created_by_user_id",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "f_k_contacts_entities_supervised_entity_id",
                table: "contacts",
                column: "supervised_entity_id",
                principalTable: "entities",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "f_k_contacts_users_created_by_user_id",
                table: "contacts",
                column: "created_by_user_id",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "f_k_faq_questions_users_answered_by_user_id",
                table: "faq_questions",
                column: "answered_by_user_id",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "f_k_faq_questions_users_submitted_by_user_id",
                table: "faq_questions",
                column: "submitted_by_user_id",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "f_k_faq_ratings_faq_questions_faq_question_id",
                table: "faq_ratings",
                column: "faq_question_id",
                principalTable: "faq_questions",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "f_k_faq_ratings_users_user_id",
                table: "faq_ratings",
                column: "user_id",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "f_k_file_libraries_file_libraries_parent_file_id",
                table: "file_libraries",
                column: "parent_file_id",
                principalTable: "file_libraries",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "f_k_file_libraries_users_uploaded_by_user_id",
                table: "file_libraries",
                column: "uploaded_by_user_id",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "f_k_file_library_permissions_entities_supervised_entity_id",
                table: "file_library_permissions",
                column: "supervised_entity_id",
                principalTable: "entities",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "f_k_file_library_permissions_file_libraries_file_library_id",
                table: "file_library_permissions",
                column: "file_library_id",
                principalTable: "file_libraries",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "f_k_file_library_permissions_users_user_id",
                table: "file_library_permissions",
                column: "user_id",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "f_k_message_attachments_messages_message_id",
                table: "message_attachments",
                column: "message_id",
                principalTable: "messages",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "f_k_message_attachments_users_uploaded_by_user_id",
                table: "message_attachments",
                column: "uploaded_by_user_id",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "f_k_messages_cases_related_case_id",
                table: "messages",
                column: "related_case_id",
                principalTable: "cases",
                principalColumn: "id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "f_k_messages_entities_related_entity_id",
                table: "messages",
                column: "related_entity_id",
                principalTable: "entities",
                principalColumn: "id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "f_k_messages_messages_parent_message_id",
                table: "messages",
                column: "parent_message_id",
                principalTable: "messages",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "f_k_messages_reports_related_report_id",
                table: "messages",
                column: "related_report_id",
                principalTable: "reports",
                principalColumn: "id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "f_k_messages_users_recipient_id",
                table: "messages",
                column: "recipient_id",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "f_k_messages_users_sender_id",
                table: "messages",
                column: "sender_id",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "f_k_password_histories_users_user_id",
                table: "password_histories",
                column: "user_id",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "f_k_password_policies_users_updated_by_user_id",
                table: "password_policies",
                column: "updated_by_user_id",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "f_k_refresh_tokens_users_user_id",
                table: "refresh_tokens",
                column: "user_id",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "f_k_reports_entities_supervised_entity_id",
                table: "reports",
                column: "supervised_entity_id",
                principalTable: "entities",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "f_k_reports_reports_original_report_id",
                table: "reports",
                column: "original_report_id",
                principalTable: "reports",
                principalColumn: "id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "f_k_reports_users_submitted_by_user_id",
                table: "reports",
                column: "submitted_by_user_id",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "f_k_role_permissions_permissions_permission_id",
                table: "role_permissions",
                column: "permission_id",
                principalTable: "permissions",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "f_k_role_permissions_roles_role_id",
                table: "role_permissions",
                column: "role_id",
                principalTable: "roles",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "f_k_user_roles_roles_role_id",
                table: "user_roles",
                column: "role_id",
                principalTable: "roles",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "f_k_user_roles_users_user_id",
                table: "user_roles",
                column: "user_id",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "f_k_user_roles_users_user_id1",
                table: "user_roles",
                column: "user_id1",
                principalTable: "users",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "f_k_users_entities_supervised_entity_id",
                table: "users",
                column: "supervised_entity_id",
                principalTable: "entities",
                principalColumn: "id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "f_k_announcement_attachments_announcements_announcement_id",
                table: "announcement_attachments");

            migrationBuilder.DropForeignKey(
                name: "f_k_announcement_histories_announcements_announcement_id",
                table: "announcement_histories");

            migrationBuilder.DropForeignKey(
                name: "f_k_announcement_histories_users_changed_by_user_id",
                table: "announcement_histories");

            migrationBuilder.DropForeignKey(
                name: "f_k_announcement_reads_announcements_announcement_id",
                table: "announcement_reads");

            migrationBuilder.DropForeignKey(
                name: "f_k_announcement_reads_users_user_id",
                table: "announcement_reads");

            migrationBuilder.DropForeignKey(
                name: "f_k_announcement_recipients_announcements_announcement_id",
                table: "announcement_recipients");

            migrationBuilder.DropForeignKey(
                name: "f_k_announcement_recipients_entities_supervised_entity_id",
                table: "announcement_recipients");

            migrationBuilder.DropForeignKey(
                name: "f_k_announcement_recipients_users_user_id",
                table: "announcement_recipients");

            migrationBuilder.DropForeignKey(
                name: "f_k_announcements_users_created_by_user_id",
                table: "announcements");

            migrationBuilder.DropForeignKey(
                name: "f_k_audit_logs_users_user_id",
                table: "audit_logs");

            migrationBuilder.DropForeignKey(
                name: "f_k_case_documents_cases_case_id",
                table: "case_documents");

            migrationBuilder.DropForeignKey(
                name: "f_k_case_documents_users_uploaded_by_user_id",
                table: "case_documents");

            migrationBuilder.DropForeignKey(
                name: "f_k_case_histories_cases_case_id",
                table: "case_histories");

            migrationBuilder.DropForeignKey(
                name: "f_k_case_histories_users_changed_by_user_id",
                table: "case_histories");

            migrationBuilder.DropForeignKey(
                name: "f_k_cases_entities_supervised_entity_id",
                table: "cases");

            migrationBuilder.DropForeignKey(
                name: "f_k_cases_users_created_by_user_id",
                table: "cases");

            migrationBuilder.DropForeignKey(
                name: "f_k_cases_users_handler_id",
                table: "cases");

            migrationBuilder.DropForeignKey(
                name: "f_k_contact_group_members_contact_groups_contact_group_id",
                table: "contact_group_members");

            migrationBuilder.DropForeignKey(
                name: "f_k_contact_group_members_contacts_contact_id",
                table: "contact_group_members");

            migrationBuilder.DropForeignKey(
                name: "f_k_contact_groups_users_created_by_user_id",
                table: "contact_groups");

            migrationBuilder.DropForeignKey(
                name: "f_k_contacts_entities_supervised_entity_id",
                table: "contacts");

            migrationBuilder.DropForeignKey(
                name: "f_k_contacts_users_created_by_user_id",
                table: "contacts");

            migrationBuilder.DropForeignKey(
                name: "f_k_faq_questions_users_answered_by_user_id",
                table: "faq_questions");

            migrationBuilder.DropForeignKey(
                name: "f_k_faq_questions_users_submitted_by_user_id",
                table: "faq_questions");

            migrationBuilder.DropForeignKey(
                name: "f_k_faq_ratings_faq_questions_faq_question_id",
                table: "faq_ratings");

            migrationBuilder.DropForeignKey(
                name: "f_k_faq_ratings_users_user_id",
                table: "faq_ratings");

            migrationBuilder.DropForeignKey(
                name: "f_k_file_libraries_file_libraries_parent_file_id",
                table: "file_libraries");

            migrationBuilder.DropForeignKey(
                name: "f_k_file_libraries_users_uploaded_by_user_id",
                table: "file_libraries");

            migrationBuilder.DropForeignKey(
                name: "f_k_file_library_permissions_entities_supervised_entity_id",
                table: "file_library_permissions");

            migrationBuilder.DropForeignKey(
                name: "f_k_file_library_permissions_file_libraries_file_library_id",
                table: "file_library_permissions");

            migrationBuilder.DropForeignKey(
                name: "f_k_file_library_permissions_users_user_id",
                table: "file_library_permissions");

            migrationBuilder.DropForeignKey(
                name: "f_k_message_attachments_messages_message_id",
                table: "message_attachments");

            migrationBuilder.DropForeignKey(
                name: "f_k_message_attachments_users_uploaded_by_user_id",
                table: "message_attachments");

            migrationBuilder.DropForeignKey(
                name: "f_k_messages_cases_related_case_id",
                table: "messages");

            migrationBuilder.DropForeignKey(
                name: "f_k_messages_entities_related_entity_id",
                table: "messages");

            migrationBuilder.DropForeignKey(
                name: "f_k_messages_messages_parent_message_id",
                table: "messages");

            migrationBuilder.DropForeignKey(
                name: "f_k_messages_reports_related_report_id",
                table: "messages");

            migrationBuilder.DropForeignKey(
                name: "f_k_messages_users_recipient_id",
                table: "messages");

            migrationBuilder.DropForeignKey(
                name: "f_k_messages_users_sender_id",
                table: "messages");

            migrationBuilder.DropForeignKey(
                name: "f_k_password_histories_users_user_id",
                table: "password_histories");

            migrationBuilder.DropForeignKey(
                name: "f_k_password_policies_users_updated_by_user_id",
                table: "password_policies");

            migrationBuilder.DropForeignKey(
                name: "f_k_refresh_tokens_users_user_id",
                table: "refresh_tokens");

            migrationBuilder.DropForeignKey(
                name: "f_k_reports_entities_supervised_entity_id",
                table: "reports");

            migrationBuilder.DropForeignKey(
                name: "f_k_reports_reports_original_report_id",
                table: "reports");

            migrationBuilder.DropForeignKey(
                name: "f_k_reports_users_submitted_by_user_id",
                table: "reports");

            migrationBuilder.DropForeignKey(
                name: "f_k_role_permissions_permissions_permission_id",
                table: "role_permissions");

            migrationBuilder.DropForeignKey(
                name: "f_k_role_permissions_roles_role_id",
                table: "role_permissions");

            migrationBuilder.DropForeignKey(
                name: "f_k_user_roles_roles_role_id",
                table: "user_roles");

            migrationBuilder.DropForeignKey(
                name: "f_k_user_roles_users_user_id",
                table: "user_roles");

            migrationBuilder.DropForeignKey(
                name: "f_k_user_roles_users_user_id1",
                table: "user_roles");

            migrationBuilder.DropForeignKey(
                name: "f_k_users_entities_supervised_entity_id",
                table: "users");

            migrationBuilder.DropPrimaryKey(
                name: "p_k_users",
                table: "users");

            migrationBuilder.DropPrimaryKey(
                name: "p_k_user_roles",
                table: "user_roles");

            migrationBuilder.DropPrimaryKey(
                name: "p_k_roles",
                table: "roles");

            migrationBuilder.DropPrimaryKey(
                name: "p_k_role_permissions",
                table: "role_permissions");

            migrationBuilder.DropPrimaryKey(
                name: "p_k_reports",
                table: "reports");

            migrationBuilder.DropPrimaryKey(
                name: "p_k_refresh_tokens",
                table: "refresh_tokens");

            migrationBuilder.DropPrimaryKey(
                name: "p_k_permissions",
                table: "permissions");

            migrationBuilder.DropPrimaryKey(
                name: "p_k_password_policies",
                table: "password_policies");

            migrationBuilder.DropPrimaryKey(
                name: "p_k_password_histories",
                table: "password_histories");

            migrationBuilder.DropPrimaryKey(
                name: "p_k_messages",
                table: "messages");

            migrationBuilder.DropPrimaryKey(
                name: "p_k_message_attachments",
                table: "message_attachments");

            migrationBuilder.DropPrimaryKey(
                name: "p_k_file_library_permissions",
                table: "file_library_permissions");

            migrationBuilder.DropPrimaryKey(
                name: "p_k_file_libraries",
                table: "file_libraries");

            migrationBuilder.DropPrimaryKey(
                name: "p_k_faq_ratings",
                table: "faq_ratings");

            migrationBuilder.DropPrimaryKey(
                name: "p_k_faq_questions",
                table: "faq_questions");

            migrationBuilder.DropPrimaryKey(
                name: "p_k_contacts",
                table: "contacts");

            migrationBuilder.DropPrimaryKey(
                name: "p_k_contact_groups",
                table: "contact_groups");

            migrationBuilder.DropPrimaryKey(
                name: "p_k_contact_group_members",
                table: "contact_group_members");

            migrationBuilder.DropPrimaryKey(
                name: "p_k_cases",
                table: "cases");

            migrationBuilder.DropPrimaryKey(
                name: "p_k_case_histories",
                table: "case_histories");

            migrationBuilder.DropPrimaryKey(
                name: "p_k_case_documents",
                table: "case_documents");

            migrationBuilder.DropPrimaryKey(
                name: "p_k_audit_logs",
                table: "audit_logs");

            migrationBuilder.DropPrimaryKey(
                name: "p_k_announcements",
                table: "announcements");

            migrationBuilder.DropPrimaryKey(
                name: "p_k_announcement_recipients",
                table: "announcement_recipients");

            migrationBuilder.DropPrimaryKey(
                name: "p_k_announcement_reads",
                table: "announcement_reads");

            migrationBuilder.DropPrimaryKey(
                name: "p_k_announcement_histories",
                table: "announcement_histories");

            migrationBuilder.DropPrimaryKey(
                name: "p_k_announcement_attachments",
                table: "announcement_attachments");

            migrationBuilder.DropPrimaryKey(
                name: "p_k_entities",
                table: "entities");

            migrationBuilder.RenameTable(
                name: "entities",
                newName: "supervised_entities");

            migrationBuilder.RenameColumn(
                name: "phone",
                table: "users",
                newName: "Phone");

            migrationBuilder.RenameColumn(
                name: "email",
                table: "users",
                newName: "Email");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "users",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "updated_at",
                table: "users",
                newName: "UpdatedAt");

            migrationBuilder.RenameColumn(
                name: "supervised_entity_id",
                table: "users",
                newName: "SupervisedEntityId");

            migrationBuilder.RenameColumn(
                name: "require_password_change",
                table: "users",
                newName: "RequirePasswordChange");

            migrationBuilder.RenameColumn(
                name: "pesel_masked",
                table: "users",
                newName: "PESEL");

            migrationBuilder.RenameColumn(
                name: "password_hash",
                table: "users",
                newName: "PasswordHash");

            migrationBuilder.RenameColumn(
                name: "password_changed_at",
                table: "users",
                newName: "LastPasswordChangeAt");

            migrationBuilder.RenameColumn(
                name: "locked_until",
                table: "users",
                newName: "LockedUntil");

            migrationBuilder.RenameColumn(
                name: "last_name",
                table: "users",
                newName: "LastName");

            migrationBuilder.RenameColumn(
                name: "last_login_at",
                table: "users",
                newName: "LastLoginAt");

            migrationBuilder.RenameColumn(
                name: "is_active",
                table: "users",
                newName: "IsActive");

            migrationBuilder.RenameColumn(
                name: "first_name",
                table: "users",
                newName: "FirstName");

            migrationBuilder.RenameColumn(
                name: "failed_login_attempts",
                table: "users",
                newName: "FailedLoginAttempts");

            migrationBuilder.RenameColumn(
                name: "created_at",
                table: "users",
                newName: "CreatedAt");

            migrationBuilder.RenameIndex(
                name: "i_x_users_supervised_entity_id",
                table: "users",
                newName: "IX_users_SupervisedEntityId");

            migrationBuilder.RenameIndex(
                name: "i_x_users_email",
                table: "users",
                newName: "IX_users_Email");

            migrationBuilder.RenameColumn(
                name: "user_id1",
                table: "user_roles",
                newName: "UserId1");

            migrationBuilder.RenameColumn(
                name: "assigned_at",
                table: "user_roles",
                newName: "AssignedAt");

            migrationBuilder.RenameColumn(
                name: "role_id",
                table: "user_roles",
                newName: "RoleId");

            migrationBuilder.RenameColumn(
                name: "user_id",
                table: "user_roles",
                newName: "UserId");

            migrationBuilder.RenameIndex(
                name: "i_x_user_roles_user_id1",
                table: "user_roles",
                newName: "IX_user_roles_UserId1");

            migrationBuilder.RenameIndex(
                name: "i_x_user_roles_role_id",
                table: "user_roles",
                newName: "IX_user_roles_RoleId");

            migrationBuilder.RenameColumn(
                name: "name",
                table: "roles",
                newName: "Name");

            migrationBuilder.RenameColumn(
                name: "description",
                table: "roles",
                newName: "Description");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "roles",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "updated_at",
                table: "roles",
                newName: "UpdatedAt");

            migrationBuilder.RenameColumn(
                name: "is_system_role",
                table: "roles",
                newName: "IsSystemRole");

            migrationBuilder.RenameColumn(
                name: "created_at",
                table: "roles",
                newName: "CreatedAt");

            migrationBuilder.RenameIndex(
                name: "i_x_roles_name",
                table: "roles",
                newName: "IX_roles_Name");

            migrationBuilder.RenameColumn(
                name: "permission_id",
                table: "role_permissions",
                newName: "PermissionId");

            migrationBuilder.RenameColumn(
                name: "role_id",
                table: "role_permissions",
                newName: "RoleId");

            migrationBuilder.RenameIndex(
                name: "i_x_role_permissions_permission_id",
                table: "role_permissions",
                newName: "IX_role_permissions_PermissionId");

            migrationBuilder.RenameColumn(
                name: "status",
                table: "reports",
                newName: "Status");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "reports",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "validation_result_path",
                table: "reports",
                newName: "ValidationResultPath");

            migrationBuilder.RenameColumn(
                name: "validated_at",
                table: "reports",
                newName: "ValidatedAt");

            migrationBuilder.RenameColumn(
                name: "supervised_entity_id",
                table: "reports",
                newName: "SupervisedEntityId");

            migrationBuilder.RenameColumn(
                name: "submitted_by_user_id",
                table: "reports",
                newName: "SubmittedByUserId");

            migrationBuilder.RenameColumn(
                name: "submitted_at",
                table: "reports",
                newName: "SubmittedAt");

            migrationBuilder.RenameColumn(
                name: "reporting_period",
                table: "reports",
                newName: "ReportingPeriod");

            migrationBuilder.RenameColumn(
                name: "report_type",
                table: "reports",
                newName: "ReportType");

            migrationBuilder.RenameColumn(
                name: "report_number",
                table: "reports",
                newName: "ReportNumber");

            migrationBuilder.RenameColumn(
                name: "original_report_id",
                table: "reports",
                newName: "OriginalReportId");

            migrationBuilder.RenameColumn(
                name: "is_correction",
                table: "reports",
                newName: "IsCorrection");

            migrationBuilder.RenameColumn(
                name: "file_path",
                table: "reports",
                newName: "FilePath");

            migrationBuilder.RenameColumn(
                name: "file_name",
                table: "reports",
                newName: "FileName");

            migrationBuilder.RenameColumn(
                name: "error_description",
                table: "reports",
                newName: "ErrorDescription");

            migrationBuilder.RenameIndex(
                name: "i_x_reports_supervised_entity_id",
                table: "reports",
                newName: "IX_reports_SupervisedEntityId");

            migrationBuilder.RenameIndex(
                name: "i_x_reports_submitted_by_user_id",
                table: "reports",
                newName: "IX_reports_SubmittedByUserId");

            migrationBuilder.RenameIndex(
                name: "i_x_reports_report_number",
                table: "reports",
                newName: "IX_reports_ReportNumber");

            migrationBuilder.RenameIndex(
                name: "i_x_reports_original_report_id",
                table: "reports",
                newName: "IX_reports_OriginalReportId");

            migrationBuilder.RenameColumn(
                name: "token",
                table: "refresh_tokens",
                newName: "Token");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "refresh_tokens",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "user_id",
                table: "refresh_tokens",
                newName: "UserId");

            migrationBuilder.RenameColumn(
                name: "revoked_by_ip",
                table: "refresh_tokens",
                newName: "RevokedByIp");

            migrationBuilder.RenameColumn(
                name: "revoked_at",
                table: "refresh_tokens",
                newName: "RevokedAt");

            migrationBuilder.RenameColumn(
                name: "revocation_reason",
                table: "refresh_tokens",
                newName: "RevocationReason");

            migrationBuilder.RenameColumn(
                name: "replaced_by_token",
                table: "refresh_tokens",
                newName: "ReplacedByToken");

            migrationBuilder.RenameColumn(
                name: "expires_at",
                table: "refresh_tokens",
                newName: "ExpiresAt");

            migrationBuilder.RenameColumn(
                name: "created_by_ip",
                table: "refresh_tokens",
                newName: "CreatedByIp");

            migrationBuilder.RenameColumn(
                name: "created_at",
                table: "refresh_tokens",
                newName: "CreatedAt");

            migrationBuilder.RenameIndex(
                name: "i_x_refresh_tokens_user_id_expires_at",
                table: "refresh_tokens",
                newName: "IX_refresh_tokens_UserId_ExpiresAt");

            migrationBuilder.RenameIndex(
                name: "i_x_refresh_tokens_token",
                table: "refresh_tokens",
                newName: "IX_refresh_tokens_Token");

            migrationBuilder.RenameColumn(
                name: "resource",
                table: "permissions",
                newName: "Resource");

            migrationBuilder.RenameColumn(
                name: "name",
                table: "permissions",
                newName: "Name");

            migrationBuilder.RenameColumn(
                name: "description",
                table: "permissions",
                newName: "Description");

            migrationBuilder.RenameColumn(
                name: "action",
                table: "permissions",
                newName: "Action");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "permissions",
                newName: "Id");

            migrationBuilder.RenameIndex(
                name: "i_x_permissions_resource_action",
                table: "permissions",
                newName: "IX_permissions_Resource_Action");

            migrationBuilder.RenameIndex(
                name: "i_x_permissions_name",
                table: "permissions",
                newName: "IX_permissions_Name");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "password_policies",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "updated_by_user_id",
                table: "password_policies",
                newName: "UpdatedByUserId");

            migrationBuilder.RenameColumn(
                name: "updated_at",
                table: "password_policies",
                newName: "UpdatedAt");

            migrationBuilder.RenameColumn(
                name: "require_uppercase",
                table: "password_policies",
                newName: "RequireUppercase");

            migrationBuilder.RenameColumn(
                name: "require_special_char",
                table: "password_policies",
                newName: "RequireSpecialChar");

            migrationBuilder.RenameColumn(
                name: "require_lowercase",
                table: "password_policies",
                newName: "RequireLowercase");

            migrationBuilder.RenameColumn(
                name: "require_digit",
                table: "password_policies",
                newName: "RequireDigit");

            migrationBuilder.RenameColumn(
                name: "min_length",
                table: "password_policies",
                newName: "MinLength");

            migrationBuilder.RenameColumn(
                name: "max_failed_attempts",
                table: "password_policies",
                newName: "MaxFailedAttempts");

            migrationBuilder.RenameColumn(
                name: "lockout_duration_minutes",
                table: "password_policies",
                newName: "LockoutDurationMinutes");

            migrationBuilder.RenameColumn(
                name: "history_count",
                table: "password_policies",
                newName: "HistoryCount");

            migrationBuilder.RenameColumn(
                name: "expiration_days",
                table: "password_policies",
                newName: "ExpirationDays");

            migrationBuilder.RenameIndex(
                name: "i_x_password_policies_updated_by_user_id",
                table: "password_policies",
                newName: "IX_password_policies_UpdatedByUserId");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "password_histories",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "user_id",
                table: "password_histories",
                newName: "UserId");

            migrationBuilder.RenameColumn(
                name: "password_hash",
                table: "password_histories",
                newName: "PasswordHash");

            migrationBuilder.RenameColumn(
                name: "created_at",
                table: "password_histories",
                newName: "CreatedAt");

            migrationBuilder.RenameIndex(
                name: "i_x_password_histories_user_id_created_at",
                table: "password_histories",
                newName: "IX_password_histories_UserId_CreatedAt");

            migrationBuilder.RenameColumn(
                name: "subject",
                table: "messages",
                newName: "Subject");

            migrationBuilder.RenameColumn(
                name: "status",
                table: "messages",
                newName: "Status");

            migrationBuilder.RenameColumn(
                name: "folder",
                table: "messages",
                newName: "Folder");

            migrationBuilder.RenameColumn(
                name: "body",
                table: "messages",
                newName: "Body");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "messages",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "thread_id",
                table: "messages",
                newName: "ThreadId");

            migrationBuilder.RenameColumn(
                name: "sent_at",
                table: "messages",
                newName: "SentAt");

            migrationBuilder.RenameColumn(
                name: "sender_id",
                table: "messages",
                newName: "SenderId");

            migrationBuilder.RenameColumn(
                name: "related_report_id",
                table: "messages",
                newName: "RelatedReportId");

            migrationBuilder.RenameColumn(
                name: "related_entity_id",
                table: "messages",
                newName: "RelatedEntityId");

            migrationBuilder.RenameColumn(
                name: "related_case_id",
                table: "messages",
                newName: "RelatedCaseId");

            migrationBuilder.RenameColumn(
                name: "recipient_id",
                table: "messages",
                newName: "RecipientId");

            migrationBuilder.RenameColumn(
                name: "read_at",
                table: "messages",
                newName: "ReadAt");

            migrationBuilder.RenameColumn(
                name: "parent_message_id",
                table: "messages",
                newName: "ParentMessageId");

            migrationBuilder.RenameColumn(
                name: "is_read",
                table: "messages",
                newName: "IsRead");

            migrationBuilder.RenameColumn(
                name: "is_cancelled",
                table: "messages",
                newName: "IsCancelled");

            migrationBuilder.RenameColumn(
                name: "cancelled_at",
                table: "messages",
                newName: "CancelledAt");

            migrationBuilder.RenameIndex(
                name: "i_x_messages_thread_id",
                table: "messages",
                newName: "IX_messages_ThreadId");

            migrationBuilder.RenameIndex(
                name: "i_x_messages_sender_id_sent_at",
                table: "messages",
                newName: "IX_messages_SenderId_SentAt");

            migrationBuilder.RenameIndex(
                name: "i_x_messages_related_report_id",
                table: "messages",
                newName: "IX_messages_RelatedReportId");

            migrationBuilder.RenameIndex(
                name: "i_x_messages_related_entity_id",
                table: "messages",
                newName: "IX_messages_RelatedEntityId");

            migrationBuilder.RenameIndex(
                name: "i_x_messages_related_case_id",
                table: "messages",
                newName: "IX_messages_RelatedCaseId");

            migrationBuilder.RenameIndex(
                name: "i_x_messages_recipient_id_is_read",
                table: "messages",
                newName: "IX_messages_RecipientId_IsRead");

            migrationBuilder.RenameIndex(
                name: "i_x_messages_parent_message_id",
                table: "messages",
                newName: "IX_messages_ParentMessageId");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "message_attachments",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "uploaded_by_user_id",
                table: "message_attachments",
                newName: "UploadedByUserId");

            migrationBuilder.RenameColumn(
                name: "uploaded_at",
                table: "message_attachments",
                newName: "UploadedAt");

            migrationBuilder.RenameColumn(
                name: "message_id",
                table: "message_attachments",
                newName: "MessageId");

            migrationBuilder.RenameColumn(
                name: "file_size",
                table: "message_attachments",
                newName: "FileSize");

            migrationBuilder.RenameColumn(
                name: "file_path",
                table: "message_attachments",
                newName: "FilePath");

            migrationBuilder.RenameColumn(
                name: "file_name",
                table: "message_attachments",
                newName: "FileName");

            migrationBuilder.RenameColumn(
                name: "content_type",
                table: "message_attachments",
                newName: "ContentType");

            migrationBuilder.RenameIndex(
                name: "i_x_message_attachments_uploaded_by_user_id",
                table: "message_attachments",
                newName: "IX_message_attachments_UploadedByUserId");

            migrationBuilder.RenameIndex(
                name: "i_x_message_attachments_message_id",
                table: "message_attachments",
                newName: "IX_message_attachments_MessageId");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "file_library_permissions",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "user_id",
                table: "file_library_permissions",
                newName: "UserId");

            migrationBuilder.RenameColumn(
                name: "supervised_entity_id",
                table: "file_library_permissions",
                newName: "SupervisedEntityId");

            migrationBuilder.RenameColumn(
                name: "role_name",
                table: "file_library_permissions",
                newName: "RoleName");

            migrationBuilder.RenameColumn(
                name: "podmiot_type",
                table: "file_library_permissions",
                newName: "PodmiotType");

            migrationBuilder.RenameColumn(
                name: "permission_type",
                table: "file_library_permissions",
                newName: "PermissionType");

            migrationBuilder.RenameColumn(
                name: "file_library_id",
                table: "file_library_permissions",
                newName: "FileLibraryId");

            migrationBuilder.RenameColumn(
                name: "can_write",
                table: "file_library_permissions",
                newName: "CanWrite");

            migrationBuilder.RenameColumn(
                name: "can_read",
                table: "file_library_permissions",
                newName: "CanRead");

            migrationBuilder.RenameColumn(
                name: "can_delete",
                table: "file_library_permissions",
                newName: "CanDelete");

            migrationBuilder.RenameIndex(
                name: "i_x_file_library_permissions_user_id",
                table: "file_library_permissions",
                newName: "IX_file_library_permissions_UserId");

            migrationBuilder.RenameIndex(
                name: "i_x_file_library_permissions_supervised_entity_id",
                table: "file_library_permissions",
                newName: "IX_file_library_permissions_SupervisedEntityId");

            migrationBuilder.RenameIndex(
                name: "i_x_file_library_permissions_file_library_id_permission_type",
                table: "file_library_permissions",
                newName: "IX_file_library_permissions_FileLibraryId_PermissionType");

            migrationBuilder.RenameColumn(
                name: "version",
                table: "file_libraries",
                newName: "Version");

            migrationBuilder.RenameColumn(
                name: "tags",
                table: "file_libraries",
                newName: "Tags");

            migrationBuilder.RenameColumn(
                name: "name",
                table: "file_libraries",
                newName: "Name");

            migrationBuilder.RenameColumn(
                name: "description",
                table: "file_libraries",
                newName: "Description");

            migrationBuilder.RenameColumn(
                name: "category",
                table: "file_libraries",
                newName: "Category");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "file_libraries",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "uploaded_by_user_id",
                table: "file_libraries",
                newName: "UploadedByUserId");

            migrationBuilder.RenameColumn(
                name: "uploaded_at",
                table: "file_libraries",
                newName: "UploadedAt");

            migrationBuilder.RenameColumn(
                name: "parent_file_id",
                table: "file_libraries",
                newName: "ParentFileId");

            migrationBuilder.RenameColumn(
                name: "is_public",
                table: "file_libraries",
                newName: "IsPublic");

            migrationBuilder.RenameColumn(
                name: "is_current_version",
                table: "file_libraries",
                newName: "IsCurrentVersion");

            migrationBuilder.RenameColumn(
                name: "file_size",
                table: "file_libraries",
                newName: "FileSize");

            migrationBuilder.RenameColumn(
                name: "file_path",
                table: "file_libraries",
                newName: "FilePath");

            migrationBuilder.RenameColumn(
                name: "file_name",
                table: "file_libraries",
                newName: "FileName");

            migrationBuilder.RenameColumn(
                name: "download_count",
                table: "file_libraries",
                newName: "DownloadCount");

            migrationBuilder.RenameColumn(
                name: "content_type",
                table: "file_libraries",
                newName: "ContentType");

            migrationBuilder.RenameIndex(
                name: "i_x_file_libraries_uploaded_by_user_id",
                table: "file_libraries",
                newName: "IX_file_libraries_UploadedByUserId");

            migrationBuilder.RenameIndex(
                name: "i_x_file_libraries_uploaded_at",
                table: "file_libraries",
                newName: "IX_file_libraries_UploadedAt");

            migrationBuilder.RenameIndex(
                name: "i_x_file_libraries_parent_file_id",
                table: "file_libraries",
                newName: "IX_file_libraries_ParentFileId");

            migrationBuilder.RenameIndex(
                name: "i_x_file_libraries_category_is_current_version",
                table: "file_libraries",
                newName: "IX_file_libraries_Category_IsCurrentVersion");

            migrationBuilder.RenameColumn(
                name: "rating",
                table: "faq_ratings",
                newName: "Rating");

            migrationBuilder.RenameColumn(
                name: "comment",
                table: "faq_ratings",
                newName: "Comment");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "faq_ratings",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "user_id",
                table: "faq_ratings",
                newName: "UserId");

            migrationBuilder.RenameColumn(
                name: "rated_at",
                table: "faq_ratings",
                newName: "RatedAt");

            migrationBuilder.RenameColumn(
                name: "faq_question_id",
                table: "faq_ratings",
                newName: "FaqQuestionId");

            migrationBuilder.RenameIndex(
                name: "i_x_faq_ratings_user_id",
                table: "faq_ratings",
                newName: "IX_faq_ratings_UserId");

            migrationBuilder.RenameIndex(
                name: "i_x_faq_ratings_faq_question_id_user_id",
                table: "faq_ratings",
                newName: "IX_faq_ratings_FaqQuestionId_UserId");

            migrationBuilder.RenameColumn(
                name: "title",
                table: "faq_questions",
                newName: "Title");

            migrationBuilder.RenameColumn(
                name: "tags",
                table: "faq_questions",
                newName: "Tags");

            migrationBuilder.RenameColumn(
                name: "status",
                table: "faq_questions",
                newName: "Status");

            migrationBuilder.RenameColumn(
                name: "content",
                table: "faq_questions",
                newName: "Content");

            migrationBuilder.RenameColumn(
                name: "category",
                table: "faq_questions",
                newName: "Category");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "faq_questions",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "view_count",
                table: "faq_questions",
                newName: "ViewCount");

            migrationBuilder.RenameColumn(
                name: "submitted_by_user_id",
                table: "faq_questions",
                newName: "SubmittedByUserId");

            migrationBuilder.RenameColumn(
                name: "submitted_at",
                table: "faq_questions",
                newName: "SubmittedAt");

            migrationBuilder.RenameColumn(
                name: "rating_count",
                table: "faq_questions",
                newName: "RatingCount");

            migrationBuilder.RenameColumn(
                name: "published_at",
                table: "faq_questions",
                newName: "PublishedAt");

            migrationBuilder.RenameColumn(
                name: "average_rating",
                table: "faq_questions",
                newName: "AverageRating");

            migrationBuilder.RenameColumn(
                name: "answered_by_user_id",
                table: "faq_questions",
                newName: "AnsweredByUserId");

            migrationBuilder.RenameColumn(
                name: "answered_at",
                table: "faq_questions",
                newName: "AnsweredAt");

            migrationBuilder.RenameColumn(
                name: "answer_content",
                table: "faq_questions",
                newName: "AnswerContent");

            migrationBuilder.RenameColumn(
                name: "anonymous_name",
                table: "faq_questions",
                newName: "AnonymousName");

            migrationBuilder.RenameColumn(
                name: "anonymous_email",
                table: "faq_questions",
                newName: "AnonymousEmail");

            migrationBuilder.RenameIndex(
                name: "i_x_faq_questions_submitted_by_user_id",
                table: "faq_questions",
                newName: "IX_faq_questions_SubmittedByUserId");

            migrationBuilder.RenameIndex(
                name: "i_x_faq_questions_submitted_at",
                table: "faq_questions",
                newName: "IX_faq_questions_SubmittedAt");

            migrationBuilder.RenameIndex(
                name: "i_x_faq_questions_status_category",
                table: "faq_questions",
                newName: "IX_faq_questions_Status_Category");

            migrationBuilder.RenameIndex(
                name: "i_x_faq_questions_published_at",
                table: "faq_questions",
                newName: "IX_faq_questions_PublishedAt");

            migrationBuilder.RenameIndex(
                name: "i_x_faq_questions_answered_by_user_id",
                table: "faq_questions",
                newName: "IX_faq_questions_AnsweredByUserId");

            migrationBuilder.RenameColumn(
                name: "position",
                table: "contacts",
                newName: "Position");

            migrationBuilder.RenameColumn(
                name: "phone",
                table: "contacts",
                newName: "Phone");

            migrationBuilder.RenameColumn(
                name: "notes",
                table: "contacts",
                newName: "Notes");

            migrationBuilder.RenameColumn(
                name: "name",
                table: "contacts",
                newName: "Name");

            migrationBuilder.RenameColumn(
                name: "mobile",
                table: "contacts",
                newName: "Mobile");

            migrationBuilder.RenameColumn(
                name: "email",
                table: "contacts",
                newName: "Email");

            migrationBuilder.RenameColumn(
                name: "department",
                table: "contacts",
                newName: "Department");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "contacts",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "updated_at",
                table: "contacts",
                newName: "UpdatedAt");

            migrationBuilder.RenameColumn(
                name: "supervised_entity_id",
                table: "contacts",
                newName: "SupervisedEntityId");

            migrationBuilder.RenameColumn(
                name: "is_primary",
                table: "contacts",
                newName: "IsPrimary");

            migrationBuilder.RenameColumn(
                name: "is_active",
                table: "contacts",
                newName: "IsActive");

            migrationBuilder.RenameColumn(
                name: "created_by_user_id",
                table: "contacts",
                newName: "CreatedByUserId");

            migrationBuilder.RenameColumn(
                name: "created_at",
                table: "contacts",
                newName: "CreatedAt");

            migrationBuilder.RenameIndex(
                name: "i_x_contacts_supervised_entity_id_is_primary",
                table: "contacts",
                newName: "IX_contacts_SupervisedEntityId_IsPrimary");

            migrationBuilder.RenameIndex(
                name: "i_x_contacts_email",
                table: "contacts",
                newName: "IX_contacts_Email");

            migrationBuilder.RenameIndex(
                name: "i_x_contacts_created_by_user_id",
                table: "contacts",
                newName: "IX_contacts_CreatedByUserId");

            migrationBuilder.RenameColumn(
                name: "name",
                table: "contact_groups",
                newName: "Name");

            migrationBuilder.RenameColumn(
                name: "description",
                table: "contact_groups",
                newName: "Description");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "contact_groups",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "created_by_user_id",
                table: "contact_groups",
                newName: "CreatedByUserId");

            migrationBuilder.RenameColumn(
                name: "created_at",
                table: "contact_groups",
                newName: "CreatedAt");

            migrationBuilder.RenameIndex(
                name: "i_x_contact_groups_created_by_user_id",
                table: "contact_groups",
                newName: "IX_contact_groups_CreatedByUserId");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "contact_group_members",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "contact_id",
                table: "contact_group_members",
                newName: "ContactId");

            migrationBuilder.RenameColumn(
                name: "contact_group_id",
                table: "contact_group_members",
                newName: "ContactGroupId");

            migrationBuilder.RenameColumn(
                name: "added_at",
                table: "contact_group_members",
                newName: "AddedAt");

            migrationBuilder.RenameIndex(
                name: "i_x_contact_group_members_contact_id",
                table: "contact_group_members",
                newName: "IX_contact_group_members_ContactId");

            migrationBuilder.RenameIndex(
                name: "i_x_contact_group_members_contact_group_id_contact_id",
                table: "contact_group_members",
                newName: "IX_contact_group_members_ContactGroupId_ContactId");

            migrationBuilder.RenameColumn(
                name: "title",
                table: "cases",
                newName: "Title");

            migrationBuilder.RenameColumn(
                name: "status",
                table: "cases",
                newName: "Status");

            migrationBuilder.RenameColumn(
                name: "priority",
                table: "cases",
                newName: "Priority");

            migrationBuilder.RenameColumn(
                name: "description",
                table: "cases",
                newName: "Description");

            migrationBuilder.RenameColumn(
                name: "category",
                table: "cases",
                newName: "Category");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "cases",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "updated_at",
                table: "cases",
                newName: "UpdatedAt");

            migrationBuilder.RenameColumn(
                name: "supervised_entity_id",
                table: "cases",
                newName: "SupervisedEntityId");

            migrationBuilder.RenameColumn(
                name: "resolved_at",
                table: "cases",
                newName: "ResolvedAt");

            migrationBuilder.RenameColumn(
                name: "is_cancelled",
                table: "cases",
                newName: "IsCancelled");

            migrationBuilder.RenameColumn(
                name: "handler_id",
                table: "cases",
                newName: "HandlerId");

            migrationBuilder.RenameColumn(
                name: "created_by_user_id",
                table: "cases",
                newName: "CreatedByUserId");

            migrationBuilder.RenameColumn(
                name: "created_at",
                table: "cases",
                newName: "CreatedAt");

            migrationBuilder.RenameColumn(
                name: "closed_at",
                table: "cases",
                newName: "ClosedAt");

            migrationBuilder.RenameColumn(
                name: "case_number",
                table: "cases",
                newName: "CaseNumber");

            migrationBuilder.RenameColumn(
                name: "cancelled_at",
                table: "cases",
                newName: "CancelledAt");

            migrationBuilder.RenameColumn(
                name: "cancellation_reason",
                table: "cases",
                newName: "CancellationReason");

            migrationBuilder.RenameIndex(
                name: "i_x_cases_supervised_entity_id_status",
                table: "cases",
                newName: "IX_cases_SupervisedEntityId_Status");

            migrationBuilder.RenameIndex(
                name: "i_x_cases_handler_id",
                table: "cases",
                newName: "IX_cases_HandlerId");

            migrationBuilder.RenameIndex(
                name: "i_x_cases_created_by_user_id",
                table: "cases",
                newName: "IX_cases_CreatedByUserId");

            migrationBuilder.RenameIndex(
                name: "i_x_cases_created_at",
                table: "cases",
                newName: "IX_cases_CreatedAt");

            migrationBuilder.RenameIndex(
                name: "i_x_cases_case_number",
                table: "cases",
                newName: "IX_cases_CaseNumber");

            migrationBuilder.RenameColumn(
                name: "description",
                table: "case_histories",
                newName: "Description");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "case_histories",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "old_status",
                table: "case_histories",
                newName: "OldStatus");

            migrationBuilder.RenameColumn(
                name: "new_status",
                table: "case_histories",
                newName: "NewStatus");

            migrationBuilder.RenameColumn(
                name: "changed_by_user_id",
                table: "case_histories",
                newName: "ChangedByUserId");

            migrationBuilder.RenameColumn(
                name: "changed_at",
                table: "case_histories",
                newName: "ChangedAt");

            migrationBuilder.RenameColumn(
                name: "change_type",
                table: "case_histories",
                newName: "ChangeType");

            migrationBuilder.RenameColumn(
                name: "case_id",
                table: "case_histories",
                newName: "CaseId");

            migrationBuilder.RenameIndex(
                name: "i_x_case_histories_changed_by_user_id",
                table: "case_histories",
                newName: "IX_case_histories_ChangedByUserId");

            migrationBuilder.RenameIndex(
                name: "i_x_case_histories_case_id_changed_at",
                table: "case_histories",
                newName: "IX_case_histories_CaseId_ChangedAt");

            migrationBuilder.RenameColumn(
                name: "description",
                table: "case_documents",
                newName: "Description");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "case_documents",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "uploaded_by_user_id",
                table: "case_documents",
                newName: "UploadedByUserId");

            migrationBuilder.RenameColumn(
                name: "uploaded_at",
                table: "case_documents",
                newName: "UploadedAt");

            migrationBuilder.RenameColumn(
                name: "file_size",
                table: "case_documents",
                newName: "FileSize");

            migrationBuilder.RenameColumn(
                name: "file_path",
                table: "case_documents",
                newName: "FilePath");

            migrationBuilder.RenameColumn(
                name: "file_name",
                table: "case_documents",
                newName: "FileName");

            migrationBuilder.RenameColumn(
                name: "document_name",
                table: "case_documents",
                newName: "DocumentName");

            migrationBuilder.RenameColumn(
                name: "content_type",
                table: "case_documents",
                newName: "ContentType");

            migrationBuilder.RenameColumn(
                name: "case_id",
                table: "case_documents",
                newName: "CaseId");

            migrationBuilder.RenameIndex(
                name: "i_x_case_documents_uploaded_by_user_id",
                table: "case_documents",
                newName: "IX_case_documents_UploadedByUserId");

            migrationBuilder.RenameIndex(
                name: "i_x_case_documents_case_id",
                table: "case_documents",
                newName: "IX_case_documents_CaseId");

            migrationBuilder.RenameColumn(
                name: "timestamp",
                table: "audit_logs",
                newName: "Timestamp");

            migrationBuilder.RenameColumn(
                name: "resource",
                table: "audit_logs",
                newName: "Resource");

            migrationBuilder.RenameColumn(
                name: "details",
                table: "audit_logs",
                newName: "Details");

            migrationBuilder.RenameColumn(
                name: "action",
                table: "audit_logs",
                newName: "Action");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "audit_logs",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "user_id",
                table: "audit_logs",
                newName: "UserId");

            migrationBuilder.RenameColumn(
                name: "resource_id",
                table: "audit_logs",
                newName: "ResourceId");

            migrationBuilder.RenameColumn(
                name: "ip_address",
                table: "audit_logs",
                newName: "IpAddress");

            migrationBuilder.RenameIndex(
                name: "i_x_audit_logs_user_id_timestamp",
                table: "audit_logs",
                newName: "IX_audit_logs_UserId_Timestamp");

            migrationBuilder.RenameIndex(
                name: "i_x_audit_logs_timestamp",
                table: "audit_logs",
                newName: "IX_audit_logs_Timestamp");

            migrationBuilder.RenameIndex(
                name: "i_x_audit_logs_resource_action",
                table: "audit_logs",
                newName: "IX_audit_logs_Resource_Action");

            migrationBuilder.RenameColumn(
                name: "title",
                table: "announcements",
                newName: "Title");

            migrationBuilder.RenameColumn(
                name: "priority",
                table: "announcements",
                newName: "Priority");

            migrationBuilder.RenameColumn(
                name: "content",
                table: "announcements",
                newName: "Content");

            migrationBuilder.RenameColumn(
                name: "category",
                table: "announcements",
                newName: "Category");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "announcements",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "updated_at",
                table: "announcements",
                newName: "UpdatedAt");

            migrationBuilder.RenameColumn(
                name: "published_at",
                table: "announcements",
                newName: "PublishedAt");

            migrationBuilder.RenameColumn(
                name: "is_published",
                table: "announcements",
                newName: "IsPublished");

            migrationBuilder.RenameColumn(
                name: "expires_at",
                table: "announcements",
                newName: "ExpiresAt");

            migrationBuilder.RenameColumn(
                name: "created_by_user_id",
                table: "announcements",
                newName: "CreatedByUserId");

            migrationBuilder.RenameColumn(
                name: "created_at",
                table: "announcements",
                newName: "CreatedAt");

            migrationBuilder.RenameIndex(
                name: "i_x_announcements_is_published_published_at",
                table: "announcements",
                newName: "IX_announcements_IsPublished_PublishedAt");

            migrationBuilder.RenameIndex(
                name: "i_x_announcements_expires_at",
                table: "announcements",
                newName: "IX_announcements_ExpiresAt");

            migrationBuilder.RenameIndex(
                name: "i_x_announcements_created_by_user_id",
                table: "announcements",
                newName: "IX_announcements_CreatedByUserId");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "announcement_recipients",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "user_id",
                table: "announcement_recipients",
                newName: "UserId");

            migrationBuilder.RenameColumn(
                name: "supervised_entity_id",
                table: "announcement_recipients",
                newName: "SupervisedEntityId");

            migrationBuilder.RenameColumn(
                name: "recipient_type",
                table: "announcement_recipients",
                newName: "RecipientType");

            migrationBuilder.RenameColumn(
                name: "podmiot_type",
                table: "announcement_recipients",
                newName: "PodmiotType");

            migrationBuilder.RenameColumn(
                name: "announcement_id",
                table: "announcement_recipients",
                newName: "AnnouncementId");

            migrationBuilder.RenameIndex(
                name: "i_x_announcement_recipients_user_id",
                table: "announcement_recipients",
                newName: "IX_announcement_recipients_UserId");

            migrationBuilder.RenameIndex(
                name: "i_x_announcement_recipients_supervised_entity_id",
                table: "announcement_recipients",
                newName: "IX_announcement_recipients_SupervisedEntityId");

            migrationBuilder.RenameIndex(
                name: "i_x_announcement_recipients_announcement_id_recipient_type",
                table: "announcement_recipients",
                newName: "IX_announcement_recipients_AnnouncementId_RecipientType");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "announcement_reads",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "user_id",
                table: "announcement_reads",
                newName: "UserId");

            migrationBuilder.RenameColumn(
                name: "read_at",
                table: "announcement_reads",
                newName: "ReadAt");

            migrationBuilder.RenameColumn(
                name: "ip_address",
                table: "announcement_reads",
                newName: "IpAddress");

            migrationBuilder.RenameColumn(
                name: "announcement_id",
                table: "announcement_reads",
                newName: "AnnouncementId");

            migrationBuilder.RenameIndex(
                name: "i_x_announcement_reads_user_id",
                table: "announcement_reads",
                newName: "IX_announcement_reads_UserId");

            migrationBuilder.RenameIndex(
                name: "i_x_announcement_reads_read_at",
                table: "announcement_reads",
                newName: "IX_announcement_reads_ReadAt");

            migrationBuilder.RenameIndex(
                name: "i_x_announcement_reads_announcement_id_user_id",
                table: "announcement_reads",
                newName: "IX_announcement_reads_AnnouncementId_UserId");

            migrationBuilder.RenameColumn(
                name: "description",
                table: "announcement_histories",
                newName: "Description");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "announcement_histories",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "changed_by_user_id",
                table: "announcement_histories",
                newName: "ChangedByUserId");

            migrationBuilder.RenameColumn(
                name: "changed_at",
                table: "announcement_histories",
                newName: "ChangedAt");

            migrationBuilder.RenameColumn(
                name: "change_type",
                table: "announcement_histories",
                newName: "ChangeType");

            migrationBuilder.RenameColumn(
                name: "announcement_id",
                table: "announcement_histories",
                newName: "AnnouncementId");

            migrationBuilder.RenameIndex(
                name: "i_x_announcement_histories_changed_by_user_id",
                table: "announcement_histories",
                newName: "IX_announcement_histories_ChangedByUserId");

            migrationBuilder.RenameIndex(
                name: "i_x_announcement_histories_announcement_id_changed_at",
                table: "announcement_histories",
                newName: "IX_announcement_histories_AnnouncementId_ChangedAt");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "announcement_attachments",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "uploaded_at",
                table: "announcement_attachments",
                newName: "UploadedAt");

            migrationBuilder.RenameColumn(
                name: "file_size",
                table: "announcement_attachments",
                newName: "FileSize");

            migrationBuilder.RenameColumn(
                name: "file_path",
                table: "announcement_attachments",
                newName: "FilePath");

            migrationBuilder.RenameColumn(
                name: "file_name",
                table: "announcement_attachments",
                newName: "FileName");

            migrationBuilder.RenameColumn(
                name: "content_type",
                table: "announcement_attachments",
                newName: "ContentType");

            migrationBuilder.RenameColumn(
                name: "announcement_id",
                table: "announcement_attachments",
                newName: "AnnouncementId");

            migrationBuilder.RenameIndex(
                name: "i_x_announcement_attachments_announcement_id",
                table: "announcement_attachments",
                newName: "IX_announcement_attachments_AnnouncementId");

            migrationBuilder.RenameColumn(
                name: "website",
                table: "supervised_entities",
                newName: "Website");

            migrationBuilder.RenameColumn(
                name: "street",
                table: "supervised_entities",
                newName: "Street");

            migrationBuilder.RenameColumn(
                name: "phone",
                table: "supervised_entities",
                newName: "Phone");

            migrationBuilder.RenameColumn(
                name: "email",
                table: "supervised_entities",
                newName: "Email");

            migrationBuilder.RenameColumn(
                name: "country",
                table: "supervised_entities",
                newName: "Country");

            migrationBuilder.RenameColumn(
                name: "city",
                table: "supervised_entities",
                newName: "City");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "supervised_entities",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "updated_at",
                table: "supervised_entities",
                newName: "UpdatedAt");

            migrationBuilder.RenameColumn(
                name: "uknf_registry_number",
                table: "supervised_entities",
                newName: "RegistryNumber");

            migrationBuilder.RenameColumn(
                name: "uknf_code",
                table: "supervised_entities",
                newName: "UKNFCode");

            migrationBuilder.RenameColumn(
                name: "r_e_g_o_n",
                table: "supervised_entities",
                newName: "REGON");

            migrationBuilder.RenameColumn(
                name: "postal_code",
                table: "supervised_entities",
                newName: "PostalCode");

            migrationBuilder.RenameColumn(
                name: "n_i_p",
                table: "supervised_entities",
                newName: "NIP");

            migrationBuilder.RenameColumn(
                name: "l_e_i",
                table: "supervised_entities",
                newName: "LEI");

            migrationBuilder.RenameColumn(
                name: "k_r_s",
                table: "supervised_entities",
                newName: "KRS");

            migrationBuilder.RenameColumn(
                name: "is_cross_border",
                table: "supervised_entities",
                newName: "IsCrossBorder");

            migrationBuilder.RenameColumn(
                name: "is_active",
                table: "supervised_entities",
                newName: "IsActive");

            migrationBuilder.RenameColumn(
                name: "entity_type",
                table: "supervised_entities",
                newName: "EntityType");

            migrationBuilder.RenameColumn(
                name: "entity_subsector",
                table: "supervised_entities",
                newName: "Subsector");

            migrationBuilder.RenameColumn(
                name: "entity_status",
                table: "supervised_entities",
                newName: "Status");

            migrationBuilder.RenameColumn(
                name: "entity_sector",
                table: "supervised_entities",
                newName: "Sector");

            migrationBuilder.RenameColumn(
                name: "entity_name",
                table: "supervised_entities",
                newName: "Name");

            migrationBuilder.RenameColumn(
                name: "entity_category",
                table: "supervised_entities",
                newName: "Category");

            migrationBuilder.RenameColumn(
                name: "created_at",
                table: "supervised_entities",
                newName: "CreatedAt");

            migrationBuilder.RenameColumn(
                name: "building_number",
                table: "supervised_entities",
                newName: "BuildingNumber");

            migrationBuilder.RenameColumn(
                name: "apartment_number",
                table: "supervised_entities",
                newName: "ApartmentNumber");

            migrationBuilder.RenameIndex(
                name: "i_x_entities_uknf_code",
                table: "supervised_entities",
                newName: "IX_supervised_entities_UKNFCode");

            migrationBuilder.AlterColumn<string>(
                name: "Phone",
                table: "users",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(250)",
                oldMaxLength: 250,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "PESEL",
                table: "users",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(250)",
                oldMaxLength: 250,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "PasswordHash",
                table: "users",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(500)",
                oldMaxLength: 500);

            migrationBuilder.AlterColumn<int>(
                name: "Status",
                table: "reports",
                type: "integer",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<int>(
                name: "Status",
                table: "messages",
                type: "integer",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<int>(
                name: "Folder",
                table: "messages",
                type: "integer",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<int>(
                name: "Status",
                table: "faq_questions",
                type: "integer",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<int>(
                name: "Status",
                table: "cases",
                type: "integer",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<int>(
                name: "Priority",
                table: "announcements",
                type: "integer",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "RegistryNumber",
                table: "supervised_entities",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(100)",
                oldMaxLength: 100,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "NIP",
                table: "supervised_entities",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(10)",
                oldMaxLength: 10,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "LEI",
                table: "supervised_entities",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(20)",
                oldMaxLength: 20,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "KRS",
                table: "supervised_entities",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(10)",
                oldMaxLength: 10,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "EntityType",
                table: "supervised_entities",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(250)",
                oldMaxLength: 250);

            migrationBuilder.AlterColumn<string>(
                name: "Subsector",
                table: "supervised_entities",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(500)",
                oldMaxLength: 500,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Status",
                table: "supervised_entities",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(250)",
                oldMaxLength: 250);

            migrationBuilder.AlterColumn<string>(
                name: "Sector",
                table: "supervised_entities",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(500)",
                oldMaxLength: 500,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Category",
                table: "supervised_entities",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(500)",
                oldMaxLength: 500,
                oldNullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_users",
                table: "users",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_user_roles",
                table: "user_roles",
                columns: new[] { "UserId", "RoleId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_roles",
                table: "roles",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_role_permissions",
                table: "role_permissions",
                columns: new[] { "RoleId", "PermissionId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_reports",
                table: "reports",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_refresh_tokens",
                table: "refresh_tokens",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_permissions",
                table: "permissions",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_password_policies",
                table: "password_policies",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_password_histories",
                table: "password_histories",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_messages",
                table: "messages",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_message_attachments",
                table: "message_attachments",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_file_library_permissions",
                table: "file_library_permissions",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_file_libraries",
                table: "file_libraries",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_faq_ratings",
                table: "faq_ratings",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_faq_questions",
                table: "faq_questions",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_contacts",
                table: "contacts",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_contact_groups",
                table: "contact_groups",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_contact_group_members",
                table: "contact_group_members",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_cases",
                table: "cases",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_case_histories",
                table: "case_histories",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_case_documents",
                table: "case_documents",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_audit_logs",
                table: "audit_logs",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_announcements",
                table: "announcements",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_announcement_recipients",
                table: "announcement_recipients",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_announcement_reads",
                table: "announcement_reads",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_announcement_histories",
                table: "announcement_histories",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_announcement_attachments",
                table: "announcement_attachments",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_supervised_entities",
                table: "supervised_entities",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_announcement_attachments_announcements_AnnouncementId",
                table: "announcement_attachments",
                column: "AnnouncementId",
                principalTable: "announcements",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_announcement_histories_announcements_AnnouncementId",
                table: "announcement_histories",
                column: "AnnouncementId",
                principalTable: "announcements",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_announcement_histories_users_ChangedByUserId",
                table: "announcement_histories",
                column: "ChangedByUserId",
                principalTable: "users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_announcement_reads_announcements_AnnouncementId",
                table: "announcement_reads",
                column: "AnnouncementId",
                principalTable: "announcements",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_announcement_reads_users_UserId",
                table: "announcement_reads",
                column: "UserId",
                principalTable: "users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_announcement_recipients_announcements_AnnouncementId",
                table: "announcement_recipients",
                column: "AnnouncementId",
                principalTable: "announcements",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_announcement_recipients_supervised_entities_SupervisedEntit~",
                table: "announcement_recipients",
                column: "SupervisedEntityId",
                principalTable: "supervised_entities",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_announcement_recipients_users_UserId",
                table: "announcement_recipients",
                column: "UserId",
                principalTable: "users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_announcements_users_CreatedByUserId",
                table: "announcements",
                column: "CreatedByUserId",
                principalTable: "users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_audit_logs_users_UserId",
                table: "audit_logs",
                column: "UserId",
                principalTable: "users",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_case_documents_cases_CaseId",
                table: "case_documents",
                column: "CaseId",
                principalTable: "cases",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_case_documents_users_UploadedByUserId",
                table: "case_documents",
                column: "UploadedByUserId",
                principalTable: "users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_case_histories_cases_CaseId",
                table: "case_histories",
                column: "CaseId",
                principalTable: "cases",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_case_histories_users_ChangedByUserId",
                table: "case_histories",
                column: "ChangedByUserId",
                principalTable: "users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_cases_supervised_entities_SupervisedEntityId",
                table: "cases",
                column: "SupervisedEntityId",
                principalTable: "supervised_entities",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_cases_users_CreatedByUserId",
                table: "cases",
                column: "CreatedByUserId",
                principalTable: "users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_cases_users_HandlerId",
                table: "cases",
                column: "HandlerId",
                principalTable: "users",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_contact_group_members_contact_groups_ContactGroupId",
                table: "contact_group_members",
                column: "ContactGroupId",
                principalTable: "contact_groups",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_contact_group_members_contacts_ContactId",
                table: "contact_group_members",
                column: "ContactId",
                principalTable: "contacts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_contact_groups_users_CreatedByUserId",
                table: "contact_groups",
                column: "CreatedByUserId",
                principalTable: "users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_contacts_supervised_entities_SupervisedEntityId",
                table: "contacts",
                column: "SupervisedEntityId",
                principalTable: "supervised_entities",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_contacts_users_CreatedByUserId",
                table: "contacts",
                column: "CreatedByUserId",
                principalTable: "users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_faq_questions_users_AnsweredByUserId",
                table: "faq_questions",
                column: "AnsweredByUserId",
                principalTable: "users",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_faq_questions_users_SubmittedByUserId",
                table: "faq_questions",
                column: "SubmittedByUserId",
                principalTable: "users",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_faq_ratings_faq_questions_FaqQuestionId",
                table: "faq_ratings",
                column: "FaqQuestionId",
                principalTable: "faq_questions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_faq_ratings_users_UserId",
                table: "faq_ratings",
                column: "UserId",
                principalTable: "users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_file_libraries_file_libraries_ParentFileId",
                table: "file_libraries",
                column: "ParentFileId",
                principalTable: "file_libraries",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_file_libraries_users_UploadedByUserId",
                table: "file_libraries",
                column: "UploadedByUserId",
                principalTable: "users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_file_library_permissions_file_libraries_FileLibraryId",
                table: "file_library_permissions",
                column: "FileLibraryId",
                principalTable: "file_libraries",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_file_library_permissions_supervised_entities_SupervisedEnti~",
                table: "file_library_permissions",
                column: "SupervisedEntityId",
                principalTable: "supervised_entities",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_file_library_permissions_users_UserId",
                table: "file_library_permissions",
                column: "UserId",
                principalTable: "users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_message_attachments_messages_MessageId",
                table: "message_attachments",
                column: "MessageId",
                principalTable: "messages",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_message_attachments_users_UploadedByUserId",
                table: "message_attachments",
                column: "UploadedByUserId",
                principalTable: "users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_messages_cases_RelatedCaseId",
                table: "messages",
                column: "RelatedCaseId",
                principalTable: "cases",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_messages_messages_ParentMessageId",
                table: "messages",
                column: "ParentMessageId",
                principalTable: "messages",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_messages_reports_RelatedReportId",
                table: "messages",
                column: "RelatedReportId",
                principalTable: "reports",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_messages_supervised_entities_RelatedEntityId",
                table: "messages",
                column: "RelatedEntityId",
                principalTable: "supervised_entities",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_messages_users_RecipientId",
                table: "messages",
                column: "RecipientId",
                principalTable: "users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_messages_users_SenderId",
                table: "messages",
                column: "SenderId",
                principalTable: "users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_password_histories_users_UserId",
                table: "password_histories",
                column: "UserId",
                principalTable: "users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_password_policies_users_UpdatedByUserId",
                table: "password_policies",
                column: "UpdatedByUserId",
                principalTable: "users",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_refresh_tokens_users_UserId",
                table: "refresh_tokens",
                column: "UserId",
                principalTable: "users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_reports_reports_OriginalReportId",
                table: "reports",
                column: "OriginalReportId",
                principalTable: "reports",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_reports_supervised_entities_SupervisedEntityId",
                table: "reports",
                column: "SupervisedEntityId",
                principalTable: "supervised_entities",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_reports_users_SubmittedByUserId",
                table: "reports",
                column: "SubmittedByUserId",
                principalTable: "users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_role_permissions_permissions_PermissionId",
                table: "role_permissions",
                column: "PermissionId",
                principalTable: "permissions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_role_permissions_roles_RoleId",
                table: "role_permissions",
                column: "RoleId",
                principalTable: "roles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_user_roles_roles_RoleId",
                table: "user_roles",
                column: "RoleId",
                principalTable: "roles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_user_roles_users_UserId",
                table: "user_roles",
                column: "UserId",
                principalTable: "users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_user_roles_users_UserId1",
                table: "user_roles",
                column: "UserId1",
                principalTable: "users",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_users_supervised_entities_SupervisedEntityId",
                table: "users",
                column: "SupervisedEntityId",
                principalTable: "supervised_entities",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }
    }
}
