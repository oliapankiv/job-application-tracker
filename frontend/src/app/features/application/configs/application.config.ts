export abstract class ApplicationConfig {
	/**
	 * @methods GET | POST
	 */
	public static readonly LIST = '/applications';

  /**
	 * @methods GET | PUT | DELETE
	 */
	public static readonly ITEM = (id: number) => `/applications/${id}`;


  /**
	 * @methods PATCH
	 */
	public static readonly STATUS = (id: number) => `/applications/${id}/status`;


  /**
	 * @methods GET
	 */
	public static readonly HISTORY = (id: number) => `/applications/${id}/history`;
}
