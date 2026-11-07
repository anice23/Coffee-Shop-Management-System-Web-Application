import { Component, OnInit, inject, ChangeDetectorRef } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { UsersService, UserDto,CreateUserRequest, UpdateUserRequest} from '../services/users';
interface User {
  id: number;
  fullName: string;
  username: string;
  role: 'Admin' | 'Cashier';
  isActive: boolean;
  isSystemUser: boolean;
}

@Component({
  selector: 'app-users',
  imports: [FormsModule],
  templateUrl: './users.html',
  styleUrl: './users.css',
})
export class Users {

  private usersService = inject(UsersService);
  private cdr = inject(ChangeDetectorRef);

  users: User[] = [];
  inactiveUsers: User[] = [];
  newPassword:string='';
  successMessage = '';

  ngOnInit(): void {
    this.loadUsers();
    this.loadInactiveUsers();

  }


// Methode N°1 : Load Users
loadUsers(): void {
  this.usersService.getUsers().subscribe({
    next: (data: UserDto[]) => {

      this.users = data.map(user => ({
        id: user.userId,
        fullName: user.fullName,
        username: user.userName,
        role: user.role,
        isActive: user.isActive,
        isSystemUser: user.isSystemUser
      }));

      this.cdr.detectChanges();
    },
    error: (error) => {
      console.error('Error loading users:', error);
    }
  });
}

// Methode N°2 : Load Inactive Users
loadInactiveUsers(): void {
  this.usersService.getInactiveUsers().subscribe({
    next: (data) => {
      this.inactiveUsers = data.map(user => ({
        id: user.userId,
        fullName: user.fullName,
        username: user.userName,
        role: user.role,
        isActive: user.isActive,
        isSystemUser: user.isSystemUser
      }));

      this.cdr.detectChanges();
    },
    error: (error) => {
      console.error('Failed to load inactive users:', error);
    }
  });
}
// Methode N°3 : Add User
addUser(): void {

  if (!this.newFullName.trim() || !this.newUsername.trim()) {
    alert('Please fill in all required fields.');
    return;
  }

  const request = {
    userName: this.newUsername.trim(),
    fullName: this.newFullName.trim(),
    password: this.newPassword,
    role: this.newRole
  };

  this.usersService.createUser(request).subscribe({
    next: (response:any) => {
      this.loadUsers();
      this.successMessage = 'User created successfully! Welcome to the team.';

      this.cancelAddForm();



      this.showAddForm = false;
      this.newFullName = '';
      this.newUsername = '';
      this.newPassword = '';
      this.newRole = 'Cashier';
    },
    error: (error) => {
      console.error('Error creating user:', error);

      alert(
        error.error?.message ?? 'Failed to create user.'
      );
    }
  });
}
  

  searchText = '';
  roleFilter: 'All' | 'Admin' | 'Cashier' = 'All';
  statusFilter: 'All' | 'Active' | 'Inactive' = 'Active';

  get filteredUsers(): User[] {

    const allUsers = [...this.users, ...this.inactiveUsers];
  
    return allUsers.filter(user => {
  
      const search = this.searchText.trim().toLowerCase();
  
      const matchesSearch =
        user.fullName.toLowerCase().includes(search) ||
        user.username.toLowerCase().includes(search);
  
      const matchesRole =
        this.roleFilter === 'All' ||
        user.role === this.roleFilter;
  
      const matchesStatus =
        this.statusFilter === 'All' ||
        (this.statusFilter === 'Active' && user.isActive) ||
        (this.statusFilter === 'Inactive' && !user.isActive);
  
      return matchesSearch && matchesRole && matchesStatus;
    });
  }

  // count Active Users
  get activeUsersCount(): number {
    return this.users.length;
  }

  // count Inactive Users
  get inactiveUsersCount(): number {
    return this.inactiveUsers.length;
  }

  // count Admins
  get adminsCount(): number {
    return [...this.users, ...this.inactiveUsers]
      .filter(user => user.role === 'Admin').length;
  }
  

  // count Cashiers
  get cashiersCount(): number {
    return [...this.users, ...this.inactiveUsers]
      .filter(user => user.role === 'Cashier').length;
  }


  showAddForm = false;
  newFullName = '';
  newUsername = '';
  newRole: 'Admin' | 'Cashier' = 'Cashier';

  openAddForm(): void {
    this.newFullName = '';
    this.newUsername = '';
    this.newRole = 'Cashier';
    this.showAddForm = true;
  }

  cancelAddForm(): void {
    this.showAddForm = false;
    this.newFullName = '';
    this.newUsername = '';
    this.newPassword = '';
    this.newRole = 'Cashier';
  }

  selectedUser: User | null = null;
  editFullName = '';
  editUsername = '';
  editRole: 'Admin' | 'Cashier' = 'Cashier';
  editNewPassword = '';
  confirmPassword = '';


  editUser(user: User): void {
    this.selectedUser = user;
    this.editFullName = user.fullName;
    this.editUsername = user.username;
    this.editRole = user.role;
  }

  saveUserChanges(): void {

    if (!this.selectedUser) return;
  
    const fullName = this.editFullName.trim();
    const username = this.editUsername.trim();
  
    if (!fullName || !username) {
      alert('Please enter the full name and username.');
      return;
    }

    // Validate password confirmation
    if (this.editNewPassword.trim()) {

       if (this.editNewPassword.length < 6) {
          alert('Password must be at least 6 characters.');
          return;
       }

     if (this.editNewPassword !== this.confirmPassword) {
       alert('New password and confirmation do not match.');
       return;
        }
    }
  else if (this.confirmPassword.trim()) {
      alert('Please enter a new password first.');
      return;
    }
  
    const alreadyExists = this.users.some(
      user =>
        user.id !== this.selectedUser!.id &&
        user.username.toLowerCase() === username.toLowerCase()
    );
  
    if (alreadyExists) {
      alert('This username already exists.');
      return;
    }
  
    const request: UpdateUserRequest = {
      userName: username,
      fullName: fullName,
      role: this.editRole
    };
  
    if (this.editNewPassword.trim()) {
      request.newPassword = this.editNewPassword;
    }
  
    this.usersService.updateUser(
      this.selectedUser.id,
      request
    ).subscribe({
  
      next: () => {
        this.successMessage = 'User updated successfully!';
        this.loadUsers();
        this.cancelEditUser();
      },
  
      error: (error) => {
        console.error('Error updating user:', error);
  
        alert(
          error.error?.message ?? 'Failed to update user.'
        );
      }
  
    });
  }  
    cancelEditUser(): void {
    this.selectedUser = null;
    this.editFullName = '';
    this.editUsername = '';
    this.editRole = 'Cashier';
    this.editNewPassword = '';
    this.confirmPassword = '';
  }

  // Methode N°4 : Desactivate User
  deactivateUser(userId: number) {
    const user = this.users.find(u => u.id === userId);
  
    if (!user) return;
  
    if (user.isSystemUser) {
      alert('System user cannot be deactivated.');
      return;
    }
  
    this.usersService.deactivateUser(userId).subscribe({
      next: (response: any) => {
        this.successMessage = response.message;
        this.refreshUsers();
      },
      error: (error) => {
        alert(error.error?.message || 'Failed to deactivate user.');
      }
    });
  }


  // Methode N°5 : Activate User
  activateUser(userId: number): void {
    this.usersService.restoreUser(userId).subscribe(
      {
        next:(response:any)=>{
          this.successMessage=response.message;
          this.refreshUsers();

        },
        error:(error)=>{
          alert(error.error?.message||'Failed to restore User')
        }
      }
        
    );
  }

  // Methode N°6 : Refresh Data 
  // Reload Active and Inactive Users
   refreshUsers(): void {
      this.loadUsers();
      this.loadInactiveUsers();
}
}
      
    
