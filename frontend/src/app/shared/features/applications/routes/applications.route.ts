import { Route } from '../../../router/classes/route/route';

export abstract class ApplicationsRoute extends Route {
	public static readonly Main = '/applications';
	public static readonly Board = '/board';

	public static Detail(id: number | string): string {
		return Route.buildURL(ApplicationsRoute.Main, id);
	}
}
