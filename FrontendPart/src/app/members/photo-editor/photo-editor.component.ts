import { Component, Input, OnInit } from '@angular/core';
import { FileUploader } from 'ng2-file-upload';
import { Member } from 'src/app/_models/member';
import { User } from 'src/app/_models/user';
import { environment } from 'src/environments/environment';
import { AccountService } from 'src/app/_services/account.service';
import { take } from 'rxjs';
import { MembersService } from 'src/app/_services/members.service';
import { Photo } from 'src/app/_models/photo';
import { ToastrService } from 'ngx-toastr';

@Component({
  selector: 'app-photo-editor',
  templateUrl: './photo-editor.component.html',
  styleUrls: ['./photo-editor.component.css']
})
export class PhotoEditorComponent implements OnInit {
  @Input() member: Member | undefined;
  uploader: FileUploader | undefined;
  hasBaseDropZoneOver = false;
  baseUrl = environment.apiUrl;
  user: User | undefined;

  constructor(private accountService: AccountService, private memberService: MembersService, private toastr: ToastrService) { 
    this.accountService.currentUser$.pipe(take(1)).subscribe({
      next: user => {
        if (user) {
          this.user = user;
        }
      }
    })
  }

  ngOnInit(): void {
    this.initializeUploader();
  }

  fileOverBaseDropZone(e: any) {
    this.hasBaseDropZoneOver = e;
  }

  setProfilePhoto(photo: Photo) {
    this.memberService.setProfilePhoto(photo.id).subscribe({
      next: () => {
        if (this.user && this.member) {
          this.user.photoUrl = photo.photoUrl;
          this.accountService.setCurrentUser(this.user);
          this.member.profilePhotoUrl = photo.photoUrl;
          this.member.photos.forEach(p => {
            if (p.isProfilePhoto) p.isProfilePhoto = false;
            if (p.id === photo.id) p.isProfilePhoto = true;
          })
        }
        this.toastr.success('The photo has successfully been set as your profile photo!');
      }
    })
  }

  deletePhoto(photoId: number) {
    this.memberService.deletePhoto(photoId).subscribe({
      next: () => {
        if (this.member) {
          this.member.photos = this.member.photos.filter(x => x.id !== photoId);
        }
        this.toastr.success('The photo has successfully been removed!');
      }
    })
  }

  initializeUploader() {
    this.uploader = new FileUploader({
      url: this.baseUrl + 'users/add-photo',
      authToken: 'Bearer ' + this.user?.token,
      isHTML5: true,
      allowedFileType: ['image'],
      removeAfterUpload: true,
      autoUpload: false,
      maxFileSize: 10 * 1024 * 1024
    });

    this.uploader.onAfterAddingFile = (file) => {
      file.withCredentials = false
    }

    this.uploader.onSuccessItem = (item, response, status, headers) => {
      if (response) {
        const photo = JSON.parse(response);
        this.member?.photos.push(photo);
        if (photo.isProfilePhoto && this.user && this.member) {
          this.user.photoUrl = photo.photoUrl;
          this.member.profilePhotoUrl = photo.photoUrl;
          this.accountService.setCurrentUser(this.user);
        }
      }
    }
  }

}
