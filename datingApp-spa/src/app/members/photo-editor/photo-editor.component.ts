import { Component, OnInit, Input, Output, EventEmitter } from '@angular/core';
import { FileUploader } from 'ng2-file-upload';
import { Photo } from 'src/app/_models/photo';
import { environment } from 'src/environments/environment';
import { AuthService } from 'src/app/_services/auth.service';
import { UserService } from 'src/app/_services/user.service';
import { AlertifyService } from 'src/app/_services/alertify.service';
import { ThrowStmt } from '@angular/compiler';

@Component({
  selector: 'app-photo-editor',
  templateUrl: './photo-editor.component.html',
  styleUrls: ['./photo-editor.component.scss']
})
export class PhotoEditorComponent implements OnInit {
  @Input() photos: Photo[];
  @Output() profilePhotoChanged = new EventEmitter<string>();

  uploader: FileUploader;
  hasBaseDropZoneOver = false;
  baseUrl = environment.apiUrl;
  currentMain: Photo;

  constructor(private authService: AuthService, private userService: UserService, private alertifyService: AlertifyService) { }

  ngOnInit() {
    this.initializeUploader();
  }

  public fileOverBase(e: any): void {
    this.hasBaseDropZoneOver = e;
  }

  initializeUploader(){
    this.uploader = new FileUploader({
      url: this.baseUrl + 'users/' + this.authService.decodedToken.nameid + '/photos/',
      authToken: 'Bearer ' + localStorage.getItem('token'),
      isHTML5: true,
      allowedFileType: ['image'],
      removeAfterUpload: true,
      autoUpload: false,
      maxFileSize: 10 * 1024 * 1024
    });

    this.uploader.onAfterAddingFile = (file) => { file.withCredentials = false; };
    this.uploader.onSuccessItem = (item, response, status, headers) => {
      if (response){
        const rsp: Photo = JSON.parse(response);
        const photo = {
          id: rsp.id,
          url: rsp.url,
          dateAdded: rsp.dateAdded,
          description: rsp.description,
          isMain: rsp.isMain
        };
        this.photos.push(photo);
        if (photo.isMain){
          this.authService.updateProfilePic(photo.url);
          this.authService.currentUser.photoUrl = photo.url;
          localStorage.setItem('User', JSON.stringify(this.authService.currentUser));

        }
      }
    };
  }

  setMainPhoto(photo: Photo){
    this.userService.setMainPhoto(this.authService.decodedToken.nameid, photo.id).subscribe(() => {
      this.currentMain = this.photos.filter( p => p.isMain === true)[0];
      this.currentMain.isMain = false;
      photo.isMain = true;
      this.authService.updateProfilePic(photo.url);
      this.authService.currentUser.photoUrl = photo.url;
      localStorage.setItem('user', JSON.stringify(this.authService.currentUser));
    }, error => {
      this.alertifyService.error(error);
    });
  }

  deletePhoro(id: number){
    this.alertifyService.confirm('Are you sure to delete the photo ??', () => {
      this.userService.deletePhoto(this.authService.decodedToken.nameid, id).subscribe( () => {
        this.photos.splice(this.photos.findIndex(p => p.id === id), 1);
        this.alertifyService.success('Photo deleted successfully.');
      }, error => {
        this.alertifyService.error(error);
      });
    });
  }

}
