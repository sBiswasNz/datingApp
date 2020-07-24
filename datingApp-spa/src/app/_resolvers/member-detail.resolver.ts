
import {Injectable} from '@angular/core';
import {Resolve, Router, ActivatedRouteSnapshot, RouterStateSnapshot} from '@angular/router';
import {User} from '../_models/user';
import { UserService } from '../_services/user.service';
import { Observable, of } from 'rxjs';
import { catchError } from 'rxjs/operators';
import { error } from 'protractor';
import { AlertifyService } from '../_services/alertify.service';



@Injectable()

export class MemberDetailResolver implements Resolve<User>{
    /**
     *
     */
    constructor(private userService: UserService, private router: Router, private alertify: AlertifyService) {}

    resolve(route: ActivatedRouteSnapshot): Observable<User> | Promise<User> {
        return this.userService.getUser(route.params['id']).pipe(
            catchError(error => {
                this.alertify.error('Problme retriving data.');
                this.router.navigate(['/members']);
                return of(null);
            })
        )
    }

    
}