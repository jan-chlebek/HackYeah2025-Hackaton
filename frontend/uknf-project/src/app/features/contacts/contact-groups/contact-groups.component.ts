import { Component, OnInit, inject, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { TableModule } from 'primeng/table';
import { ButtonModule } from 'primeng/button';
import { InputTextModule } from 'primeng/inputtext';
import { BreadcrumbModule } from 'primeng/breadcrumb';
import { DialogModule } from 'primeng/dialog';
import { TextareaModule } from 'primeng/textarea';
import { MultiSelectModule } from 'primeng/multiselect';
import { MenuItem } from 'primeng/api';
import { ContactGroupService, ContactGroupListItem, ContactGroup, ContactGroupMember, CreateContactGroupRequest, UpdateContactGroupRequest, ContactGroupListResponse } from '../../../services/contact-group.service';
import { ContactService, ContactListItem } from '../../../services/contact.service';
import { AuthService } from '../../../services/auth.service';

@Component({
  selector: 'app-contact-groups',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    TableModule,
    ButtonModule,
    InputTextModule,
    BreadcrumbModule,
    DialogModule,
    TextareaModule,
    MultiSelectModule
  ],
  templateUrl: './contact-groups.component.html',
  styleUrls: ['./contact-groups.component.css']
})
export class ContactGroupsComponent implements OnInit {
  private groupService = inject(ContactGroupService);
  private contactService = inject(ContactService);
  private authService = inject(AuthService);
  private router = inject(Router);
  private cdr = inject(ChangeDetectorRef);

  // Breadcrumb - will be built based on permissions
  breadcrumbItems: MenuItem[] = [];
  home: MenuItem = { icon: 'pi pi-home', routerLink: '/' };

  // Table data
  groups: ContactGroupListItem[] = [];
  loading = false;
  totalRecords = 0;
  
  // Pagination
  page = 1;
  pageSize = 20;
  first = 0;
  pageSizeOptions = [10, 20, 50, 100];

  // Dialogs
  showCreateDialog = false;
  showEditDialog = false;
  showDeleteDialog = false;
  showMembersDialog = false;
  showAddMembersDialog = false;

  // Create/Edit form
  groupForm: CreateContactGroupRequest | UpdateContactGroupRequest = {
    name: '',
    description: null
  };
  editingGroupId: number | null = null;

  // Group to delete
  groupToDelete: ContactGroupListItem | null = null;

  // Members management
  selectedGroup: ContactGroup | null = null;
  groupMembers: ContactGroupMember[] = [];
  availableContacts: ContactListItem[] = [];
  selectedContactIds: number[] = [];

  ngOnInit(): void {
    // Build breadcrumb based on permissions
    const items: MenuItem[] = [];
    
    // Only show 'Adresaci' breadcrumb if user has elevated permissions
    if (this.authService.hasElevatedPermissions()) {
      items.push({ label: 'Adresaci', routerLink: '/contacts' });
    }
    
    items.push({ label: 'Grupy kontaktów' });
    this.breadcrumbItems = items;
    
    this.loadGroups();
  }

  loadGroups(): void {
    this.loading = true;
    console.log('Loading contact groups');
    
    this.groupService.getContactGroups(this.page, this.pageSize).subscribe({
      next: (response: ContactGroupListResponse) => {
        console.log('Groups loaded successfully:', response);
        this.groups = response.data;
        this.totalRecords = response.pagination.totalCount;
        this.loading = false;
        this.cdr.detectChanges();
      },
      error: (error: any) => {
        console.error('Error loading groups:', error);
        this.loading = false;
        this.cdr.detectChanges();
      }
    });
  }

  onPageChange(event: any): void {
    this.page = event.page + 1;
    this.pageSize = event.rows;
    this.first = event.first;
    this.loadGroups();
  }

  openCreateDialog(): void {
    this.groupForm = { name: '', description: null };
    this.showCreateDialog = true;
  }

  openEditDialog(group: ContactGroupListItem): void {
    this.editingGroupId = group.id;
    this.groupForm = {
      name: group.name,
      description: group.description
    };
    this.showEditDialog = true;
  }

  createGroup(): void {
    this.groupService.createContactGroup(this.groupForm as CreateContactGroupRequest).subscribe({
      next: () => {
        console.log('Group created successfully');
        this.showCreateDialog = false;
        this.loadGroups();
      },
      error: (error: any) => {
        console.error('Error creating group:', error);
      }
    });
  }

  updateGroup(): void {
    if (this.editingGroupId) {
      this.groupService.updateContactGroup(this.editingGroupId, this.groupForm as UpdateContactGroupRequest).subscribe({
        next: () => {
          console.log('Group updated successfully');
          this.showEditDialog = false;
          this.editingGroupId = null;
          this.loadGroups();
        },
        error: (error: any) => {
          console.error('Error updating group:', error);
        }
      });
    }
  }

  confirmDelete(group: ContactGroupListItem): void {
    this.groupToDelete = group;
    this.showDeleteDialog = true;
  }

  deleteGroup(): void {
    if (this.groupToDelete) {
      this.groupService.deleteContactGroup(this.groupToDelete.id).subscribe({
        next: () => {
          console.log('Group deleted successfully');
          this.showDeleteDialog = false;
          this.groupToDelete = null;
          this.loadGroups();
        },
        error: (error) => {
          console.error('Error deleting group:', error);
          this.showDeleteDialog = false;
        }
      });
    }
  }

  viewMembers(groupId: number): void {
    this.groupService.getContactGroupById(groupId).subscribe({
      next: (group) => {
        this.selectedGroup = group;
        this.groupMembers = group.members || [];
        this.showMembersDialog = true;
      },
      error: (error) => {
        console.error('Error loading group members:', error);
      }
    });
  }

  openAddMembersDialog(): void {
    if (this.selectedGroup) {
      this.groupService.getAvailableContacts(this.selectedGroup.id).subscribe({
        next: (contacts) => {
          this.availableContacts = contacts;
          this.selectedContactIds = [];
          this.showAddMembersDialog = true;
        },
        error: (error) => {
          console.error('Error loading available contacts:', error);
        }
      });
    }
  }

  addMembers(): void {
    if (this.selectedGroup && this.selectedContactIds.length > 0) {
      this.groupService.addMembers(this.selectedGroup.id, { contactIds: this.selectedContactIds }).subscribe({
        next: () => {
          console.log('Members added successfully');
          this.showAddMembersDialog = false;
          this.viewMembers(this.selectedGroup!.id);
        },
        error: (error) => {
          console.error('Error adding members:', error);
        }
      });
    }
  }

  removeMember(memberId: number): void {
    if (this.selectedGroup) {
      this.groupService.removeMember(this.selectedGroup.id, memberId).subscribe({
        next: () => {
          console.log('Member removed successfully');
          this.viewMembers(this.selectedGroup!.id);
        },
        error: (error) => {
          console.error('Error removing member:', error);
        }
      });
    }
  }

  backToContacts(): void {
    this.router.navigate(['/contacts']);
  }
}
