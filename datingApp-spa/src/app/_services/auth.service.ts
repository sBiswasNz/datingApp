import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { BehaviorSubject } from 'rxjs';
import { map } from 'rxjs/operators';
import { JwtHelperService } from '@auth0/angular-jwt';
import { environment } from 'src/environments/environment';
import { User } from '../_models/user';


@Injectable({
  providedIn: 'root'
})


export class AuthService {
  baseUrl = environment.apiUrl + 'auth/';
  jwtHelper = new JwtHelperService();
  decodedToken: any;
  currentUser: User;
  photoUrl = new BehaviorSubject<string>('../assets/images/user.png');
  currentPhotoUrl = this.photoUrl.asObservable();


  constructor( private http: HttpClient) { }

  updateProfilePic(photoUrl: string){
    this.photoUrl.next(photoUrl);
  }

  login(model: any){
    console.log(model);
    return this.http.post(this.baseUrl + 'login', model)
      .pipe(
        map ((response: any) => {
          const resp = response;
          if (resp){
            localStorage.setItem('token', resp.token);
            localStorage.setItem('user', JSON.stringify(resp.user));
            this.decodedToken = this.jwtHelper.decodeToken(resp.token);
            this.currentUser = resp.user;
            this.updateProfilePic(this.currentUser.photoUrl);
          }
        })
      );
  }

  register(model: any){
    return this.http.post(this.baseUrl + 'register', model);
  }

  loggedIn() {
    const token = localStorage.getItem('token');
    return !this.jwtHelper.isTokenExpired(token);

  }


}
