export abstract class Route {
	public static buildURL(...parts: Array<string | number>): string {
		return parts.filter(Boolean).join('/');
	}
}
