import { Route } from '../../../router/classes/route/route';

export abstract class AuthRoute extends Route {
	public static readonly Login = '/login';
}
