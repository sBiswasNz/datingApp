
import {Injectable} from '@angular/core';
import {Resolve, Router, ActivatedRouteSnapshot, RouterStateSnapshot} from '@angular/router';
import {User} from '../_models/user';
import { UserService } from '../_services/user.service';
import { Observable, of } from 'rxjs';
import { catchError } from 'rxjs/operators';
import { error } from 'protractor';
import { AlertifyService } from '../_services/alertify.service';
import { AuthService } from '../_services/auth.service';



@Injectable()

export class MemberEditResolver implements Resolve<User>{
    /**
     *
     */
    constructor(private userService: UserService, private router: Router,
                private alertify: AlertifyService, private authService: AuthService) {}

    resolve(route: ActivatedRouteSnapshot): Observable<User> | Promise<User> {
        return this.userService.getUser(this.authService.decodedToken.nameid).pipe(
            catchError(error => {
                this.alertify.error('Problme retriving your data.');
                this.router.navigate(['/members']);
                return of(null);
            })
        );
    }
}
