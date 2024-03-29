import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environment } from 'src/environments/environment';
import { Member } from '../_models/member';
import { map, of, take } from 'rxjs';
import { UserParamsForPagination } from '../_models/userParamsForPagination';
import { AccountService } from './account.service';
import { User } from '../_models/user';
import { getPaginationHeaders, getPaginatedResult } from './paginationHelpers';

@Injectable({
  providedIn: 'root',
})
export class MembersService {
  baseUrl = environment.apiUrl;
  members: Member[] = [];
  cachedMembers = new Map();
  userParams: UserParamsForPagination | undefined;
  user: User | undefined;

  constructor(private http: HttpClient, private accountService: AccountService) {
    this.accountService.currentUser$.pipe(take(1)).subscribe({
      next: user => {
        if (user) {
          this.userParams = new UserParamsForPagination(user);
          this.user = user;
        }
      }
    })
  }

  getUserParams() {
    return this.userParams;
  }

  setUserParams(userParams: UserParamsForPagination) {
    this.userParams = userParams;
  }

  resetUserParams() {
    if (this.user) {
      this.userParams = new UserParamsForPagination(this.user);
      return this.userParams;
    }
    return;
  }

  getMembers(userParams: UserParamsForPagination) {
    const response = this.cachedMembers.get(Object.values(userParams).join('-'));
    
    if (response) return of(response);

    let params = getPaginationHeaders(userParams.pageNumber, userParams.itemsPerPage);

    params = params.append('minAge', userParams.minAge);
    params = params.append('maxAge', userParams.maxAge);
    params = params.append('gender', userParams.gender);
    params = params.append('orderBy', userParams.orderBy);

    return getPaginatedResult<Member[]>(this.baseUrl + 'users', params, this.http).pipe(
      map(response => {
        this.cachedMembers.set(Object.values(userParams).join('-'), response);
        return response;
      })
    );
  }

  getMember(username: string) {
    const member = [...this.cachedMembers.values()].reduce((initialArray, currentArray) => initialArray.concat(currentArray.result), [])
                    .find((member: Member) => member.userName === username);
    if (member) return of(member);

    return this.http.get<Member>(this.baseUrl + 'users/' + username);
  }

  updateMemberInfo(memberInfo: Member) {
    return this.http.put(this.baseUrl + 'users', memberInfo).pipe(
      map(() => {
        const index = this.members.indexOf(memberInfo);
        this.members[index] = {...this.members[index], ...memberInfo};
      })
    );
  }

  setProfilePhoto(photoId: number) {
    return this.http.put(this.baseUrl + 'users/set-profile-photo/' + photoId, {});
  }

  deletePhoto(photoId: number) {
    return this.http.delete(this.baseUrl + 'users/delete-photo/' + photoId);
  }

  addToFavourite(username: string) {
    return this.http.post(this.baseUrl + 'favourites/' + username, {});
  }

  getFavouriteList(predicate: string, pageNumber: number, pageSize: number) {
    let params = getPaginationHeaders(pageNumber, pageSize);
    params = params.append('predicate', predicate);
    
    return getPaginatedResult<Member[]>(this.baseUrl + 'favourites', params, this.http);
  }

  // getHttpOptions() {
  //   const userString = localStorage.getItem('user');
  //   if (!userString) return;
  //   const user = JSON.parse(userString);
  //   return {
  //     headers: new HttpHeaders({
  //       Authorization: 'Bearer ' + user.token,
  //     }),
  //   };
  // }          This part will now be handled by the 'JwtAuthorizerInterceptor'
}
