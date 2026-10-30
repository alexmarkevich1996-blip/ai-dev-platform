declare const brand: unique symbol

/**
 * A string identifier that cannot be passed where another kind is expected.
 *
 * Entity slices may not import each other, so a slice that refers to another
 * entity holds a plain identifier rather than an embedded object — the same
 * decision the backend makes when Projects stores an Employee identifier.
 * Branding keeps those identifiers from being mixed up.
 */
export type Brand<TName extends string> = string & { readonly [brand]: TName }
