import { Component, OnInit } from '@angular/core';
import { AuthService } from '../_services/auth.service';
import { Observable } from 'rxjs';
import { User } from '../_models/user';
import { Router } from '@angular/router';
import { ToastrService } from 'ngx-toastr';

@Component({
  selector: 'app-nav',
  templateUrl: './nav.component.html',
  styleUrls: ['./nav.component.css']
})
export class NavComponent implements OnInit {
  model: any = {}

  constructor(public authService: AuthService, private router: Router,
    private toastr: ToastrService) { }

  ngOnInit(): void {
  }

  login() {
    this.authService.login(this.model).subscribe(response => {
      console.log(response);
      this.router.navigateByUrl('/system-containers');
    })
  }

  logout() {
    this.authService.logout();
    this.router.navigateByUrl('/')
  }
}