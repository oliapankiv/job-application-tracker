export abstract class ContactConfig {
   /**
	 * @methods GET | POST
	 */
	public static readonly LIST = (id: number): string => `/applications/${id}/contacts`;

  /**
	 * @methods POST
	 */
	public static readonly ITEM = (id: number): string => `/contacts/${id}`;
}
