import { User } from "./user";

export class UserParamsForPagination {
    pageNumber = 1;
    itemsPerPage = 5;
    gender: string;
    minAge = 18;
    maxAge = 99;
    orderBy = 'lastActive';

    constructor (user: User) {
        this.gender = user.gender === 'male' ? 'female' : 'male';
    }
}