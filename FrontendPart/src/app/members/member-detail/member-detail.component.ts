import { Component, OnDestroy, OnInit, ViewChild } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { NgxGalleryAnimation, NgxGalleryImage, NgxGalleryOptions } from '@kolkov/ngx-gallery';
import { TabDirective, TabsetComponent } from 'ngx-bootstrap/tabs';
import { ToastrService } from 'ngx-toastr';
import { take } from 'rxjs';
import { Member } from 'src/app/_models/member';
import { Message } from 'src/app/_models/message';
import { User } from 'src/app/_models/user';
import { AccountService } from 'src/app/_services/account.service';
import { MembersService } from 'src/app/_services/members.service';
import { MessagesService } from 'src/app/_services/messages.service';
import { PresenceService } from 'src/app/_services/presence.service';

@Component({
  selector: 'app-member-detail',
  templateUrl: './member-detail.component.html',
  styleUrls: ['./member-detail.component.css']
})
export class MemberDetailComponent implements OnInit, OnDestroy {
  @ViewChild('memberTabs', {static: true}) memberTabs?: TabsetComponent;
  member: Member = {} as Member;
  galleryOptions: NgxGalleryOptions[] = [];
  galleryImages: NgxGalleryImage[] = [];
  activatedTab?: TabDirective;
  messages: Message[] = [];
  user?: User;

  constructor(private route: ActivatedRoute, private messageService: MessagesService, 
    public presenceService: PresenceService, private accountService: AccountService, 
    private router: Router, private memberService: MembersService, private toastr: ToastrService) {
      this.accountService.currentUser$.pipe(take(1)).subscribe({
        next: user => {
          if (user) this.user = user;
        }
      })
  }

  ngOnInit(): void {
    this.route.data.subscribe({
      next: data => this.member = data['member']
    });
    
    this.route.queryParams.subscribe({
      next: params => {
        params['tab'] && this.onSelectTab(params['tab']);
      }
    });
    
    this.galleryOptions = [
      {
        width: '500px',
        height: '500px',
        imagePercent: 100,
        thumbnailsColumns: 4,
        imageAnimation: NgxGalleryAnimation.Slide,
        preview: false
      }
    ];

    this.galleryImages = this.getImages();

  }

  ngOnDestroy(): void {
    this.messageService.stopHubConnection();
  }

  onSelectTab(tabHeading: string) {
    if (this.memberTabs) {
      this.memberTabs.tabs.find(tab => tab.heading === tabHeading)!.active = true;
    }
  }

  onActivatedTab(data: TabDirective) {
    this.activatedTab = data;
    if (this.activatedTab.heading === 'Messages' && this.user) {
      this.messageService.createHubConnection(this.user, this.member.userName)
    } else {
      this.messageService.stopHubConnection();
      this.removeTabInfo();
    }
  }

  removeTabInfo() {
    const currentUrl = this.router.url;
    const urlTree = this.router.parseUrl(currentUrl);
    const queryParams = { ...urlTree.queryParams };
    
    if (queryParams['tab']) {
      delete queryParams['tab'];
    }

    const newUrl = this.router.createUrlTree([], queryParams).toString();
    this.router.navigateByUrl(newUrl);
  }

  loadMessagesThread() {
    if (this.member) {
      this.messageService.getMessagesThread(this.member.userName).subscribe({
        next: messages => this.messages = messages
      });
    }
  }

  getImages() {
    if(!this.member) return [];
    const imageUrls = [];
    for(const photo of this.member.photos) {
      imageUrls.push({
        small: photo.photoUrl,
        medium: photo.photoUrl,
        big: photo.photoUrl
      })
    }
    return imageUrls;
  }

  addToFavourite(member: Member) {
    this.memberService.addToFavourite(member.userName).subscribe({
      next: () => this.toastr.success(member.nickname + ` has been successfully added to your Favourite!`)
    });
  }

}
