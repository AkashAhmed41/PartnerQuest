import { Photo } from './photo';

export interface Member {
  id: number;
  userName: string;
  profilePhotoUrl: string;
  age: number;
  nickname: string;
  created: string;
  lastActive: string;
  gender: string;
  about: string;
  lookingFor: string;
  interests: string;
  city: string;
  country: string;
  photos: Photo[];
}
