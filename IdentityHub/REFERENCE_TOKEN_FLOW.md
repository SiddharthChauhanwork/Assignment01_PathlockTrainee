# Reference Token Pattern in IdentityHub

## What are Reference Tokens?

**Reference Tokens** (also called **Opaque Tokens**) are tokens that have no intrinsic meaning - they are random strings that serve as references to data stored on the server. This is in contrast to **JWT tokens** which are self-contained and can be validated without a database lookup.

## Architecture in IdentityHub

### Token Flow Overview

```
┌─────────────────────────────────────────────────────────────────┐
│                    1. Token Generation                          │
└─────────────────────────────────────────────────────────────────┘

Client Application                    IdentityHub Server
	  │                                      │
	  │  POST /auth/token                    │
	  │  {                                   │
	  │    "clientId": "myapp",              │
	  │    "clientSecret": "secret",         │
	  │    "email": "user@example.com"       │
	  │  }                                   │
	  │─────────────────────────────────────>│
	  │                                      │
	  │                                      │  • Generate random bytes
	  │                                      │  • referenceToken = Base64(random)
	  │                                      │  • tokenHash = SHA256(referenceToken)
	  │                                      │  • Store in AccessToken table:
	  │                                      │    - TokenHash
	  │                                      │    - Email
	  │                                      │    - ExpiresAt (15 min)
	  │                                      │    - ClientId
	  │                                      │
	  │  Response:                           │
	  │  {                                   │
	  │    "accessToken": "wK7x...",  <──────┤  Return UNHASHED referenceToken
	  │    "refreshToken": "pL4y...",        │
	  │    "tokenType": "Bearer",            │
	  │    "expiresIn": 900                  │
	  │  }                                   │
	  │<─────────────────────────────────────│

┌─────────────────────────────────────────────────────────────────┐
│                    2. Token Validation                          │
└─────────────────────────────────────────────────────────────────┘

Resource Server                       IdentityHub Server
	  │                                      │
	  │  POST /auth/validate                 │
	  │  "wK7x..."                           │
	  │─────────────────────────────────────>│
	  │                                      │
	  │                                      │  • Hash incoming token
	  │                                      │  • Search DB for matching hash
	  │                                      │  • Check: NOT expired
	  │                                      │  • Check: NOT revoked
	  │                                      │  • Check: Client is active
	  │                                      │
	  │  { "valid": true }                   │
	  │<─────────────────────────────────────│

┌─────────────────────────────────────────────────────────────────┐
│                    3. Token Revocation                          │
└─────────────────────────────────────────────────────────────────┘

Client Application                    IdentityHub Server
	  │                                      │
	  │  POST /auth/revoke                   │
	  │  "wK7x..."                           │
	  │─────────────────────────────────────>│
	  │                                      │
	  │                                      │  • Hash incoming token
	  │                                      │  • Find AccessToken record
	  │                                      │  • Set RevokedAt = NOW
	  │                                      │  • Update database
	  │                                      │
	  │  {                                   │
	  │    "message": "Token revoked"        │
	  │  }                                   │
	  │<─────────────────────────────────────│
```

## Database Storage

### AccessToken Table

| Column              | Type     | Description                                    |
|---------------------|----------|------------------------------------------------|
| Id                  | Guid     | Primary key                                    |
| TokenHash           | string   | SHA256 hash of the reference token             |
| ApplicationClientId | Guid     | Foreign key to client                          |
| Email               | string   | User email associated with the token           |
| ExpiresAt           | DateTime | Token expiration time (15 minutes from issue)  |
| RevokedAt           | DateTime?| Null if active, set when revoked               |
| CreatedAt           | DateTime | Token creation timestamp                       |

**Key Point:** The actual reference token (`wK7x...`) is NEVER stored in the database. Only its hash is stored.

## Why Reference Tokens?

### Advantages:
1. **Instant Revocation**: Token can be invalidated immediately by updating a database record
2. **Smaller Payload**: Client only sends a short random string (no JWT overhead)
3. **Server Control**: All validation logic happens on the server
4. **No Token Expiry in Token**: Expiry is checked in database, can be updated dynamically
5. **Audit Trail**: Every validation can be logged in the database

### Disadvantages:
1. **Database Lookup Required**: Every validation requires a database query
2. **Not Stateless**: Server must maintain state (database records)
3. **Scalability**: High traffic requires good database performance
4. **No Offline Validation**: Resource servers must always call IdentityHub

## Implementation Details

### Token Generation (TokenService.cs)

```csharp
// 1. Generate cryptographically secure random token
private static string GenerateSecureToken()
{
	var bytes = RandomNumberGenerator.GetBytes(32);
	return Convert.ToBase64String(bytes);
}

// 2. Create reference token
var referenceToken = GenerateSecureToken();

// 3. Hash it for storage
var tokenHash = _refreshTokenHasher.HashToken(referenceToken);

// 4. Store hash in database
var accessTokenEntity = new AccessToken
{
	TokenHash = tokenHash,  // Hashed version
	ExpiresAt = DateTime.UtcNow.AddMinutes(15),
	// ... other fields
};

// 5. Return UNHASHED token to client
return new TokenResponseDto
{
	AccessToken = referenceToken,  // Raw token
	// ...
};
```

### Token Validation

```csharp
public async Task<bool> ValidateAccessTokenAsync(string referenceToken)
{
	// 1. Hash the incoming token
	var tokenHash = _refreshTokenHasher.HashToken(referenceToken);

	// 2. Look up in database by hash
	var storedToken = await _accessTokenRepository
		.GetByTokenHashAsync(tokenHash);

	if (storedToken == null) return false;

	// 3. Check expiration
	if (storedToken.IsExpired) return false;

	// 4. Check revocation
	if (storedToken.IsRevoked) return false;

	// 5. Check client status
	if (!storedToken.ApplicationClient.IsActive) return false;

	return true;
}
```

### Token Revocation

```csharp
public async Task RevokeAccessTokenAsync(string referenceToken)
{
	// 1. Hash the token
	var tokenHash = _refreshTokenHasher.HashToken(referenceToken);

	// 2. Find in database
	var storedToken = await _accessTokenRepository
		.GetByTokenHashAsync(tokenHash);

	if (storedToken == null)
		throw new AccessTokenNotFoundException();

	if (storedToken.IsRevoked)
		throw new AccessTokenRevokedException();

	// 3. Mark as revoked
	storedToken.RevokedAt = DateTime.UtcNow;

	// 4. Save to database
	await _accessTokenRepository.UpdateAsync(storedToken);
}
```

## API Usage Examples

### 1. Generate Token

**Request:**
```http
POST /auth/token
Content-Type: application/json

{
  "clientId": "mobile-app",
  "clientSecret": "my-super-secret-key",
  "email": "john@example.com"
}
```

**Response:**
```json
{
  "accessToken": "wK7xYZaBcDeFgHiJkLmNoPqRsTuVwXyZ",
  "refreshToken": "pL4yXZaBcDeFgHiJkLmNoPqRsTuVwXyZ",
  "tokenType": "Bearer",
  "expiresIn": 900
}
```

**Note:** The `accessToken` in the response is the **reference token** that you'll use for validation and revocation.

### 2. Validate Token (by Resource Server)

**Request:**
```http
POST /auth/validate
Content-Type: application/json

"wK7xYZaBcDeFgHiJkLmNoPqRsTuVwXyZ"
```

**Response (Valid):**
```json
{
  "valid": true
}
```

**Response (Invalid):**
```http
HTTP 401 Unauthorized
{
  "message": "Invalid, expired, or revoked reference token."
}
```

### 3. Revoke Token

**Request:**
```http
POST /auth/revoke
Content-Type: application/json

"wK7xYZaBcDeFgHiJkLmNoPqRsTuVwXyZ"
```

**Response:**
```json
{
  "message": "Reference token revoked successfully."
}
```

### 4. Refresh Token

**Request:**
```http
POST /auth/refresh
Content-Type: application/json

{
  "refreshToken": "pL4yXZaBcDeFgHiJkLmNoPqRsTuVwXyZ"
}
```

**Response:**
```json
{
  "accessToken": "NEW-wK7xYZaBcDeFgHiJkLmNoPqRs",
  "refreshToken": "NEW-pL4yXZaBcDeFgHiJkLmNoP",
  "tokenType": "Bearer",
  "expiresIn": 900
}
```

## Security Considerations

### Why Hash Tokens in Database?

Even though reference tokens are random and meaningless, we hash them before storage because:

1. **Database Breach Protection**: If someone gains read access to the database, they can't use the hashed tokens
2. **Defense in Depth**: Multiple layers of security
3. **Best Practice**: OAuth2 RFC 7662 recommends hashing tokens at rest

### Token Lifecycle

```
┌────────────┐
│  Generated │  RevokedAt = NULL, NOW < ExpiresAt
└─────┬──────┘
	  │
	  v
┌────────────┐
│   Active   │  Can be validated, used for API access
└─────┬──────┘
	  │
	  ├─────> [Time Passes > 15 min] ──> ┌──────────┐
	  │                                   │ Expired  │
	  │                                   └──────────┘
	  │
	  └─────> [Manual Revocation]    ──> ┌──────────┐
										  │ Revoked  │
										  └──────────┘
```

### Comparison with JWT

| Aspect              | Reference Token (IdentityHub) | JWT                           |
|---------------------|-------------------------------|-------------------------------|
| Size                | Small (~32-64 bytes)          | Large (200-2000 bytes)        |
| Validation          | Database lookup required      | Signature verification only   |
| Revocation          | Immediate (update DB)         | Difficult (needs blacklist)   |
| Stateless           | No (database required)        | Yes                           |
| Expiry              | Checked in DB                 | Embedded in token             |
| Performance         | DB query per validation       | Fast (no DB needed)           |
| Use Case            | High security, instant revoke | Distributed, scalable systems |

## Best Practices

1. **Always validate on Resource Servers**: Never trust client-side validation
2. **Use HTTPS**: Reference tokens are bearer tokens - anyone with the token can use it
3. **Short Expiry**: Keep access tokens short-lived (15 minutes default)
4. **Use Refresh Tokens**: Reduce frequent authentication with long-lived refresh tokens
5. **Monitor Database Performance**: Token validation queries should be fast (index on TokenHash)
6. **Implement Rate Limiting**: Prevent brute force token guessing
7. **Log Validation Attempts**: Track suspicious activity

## Migration to JWT (Optional Enhancement)

If you need both reference tokens AND JWTs, you can:

1. Generate a JWT containing claims
2. Also generate a reference token (JTI - JWT ID)
3. Store the JTI in the database for revocation checks
4. Resource servers can validate JWT offline
5. Critical operations can verify JTI hasn't been revoked

This gives you the best of both worlds: fast stateless validation with optional revocation capability.

## Troubleshooting

### Token Validation Fails

**Check:**
1. Is the token expired? (Check `ExpiresAt` in database)
2. Is the token revoked? (Check `RevokedAt` is NULL)
3. Is the client active? (Check `ApplicationClient.IsActive`)
4. Is the token hash correct? (Ensure you're sending the raw token, not the hash)

### Token Not Found

**Common causes:**
1. Token was never generated (check generation response)
2. Token was deleted from database (check database)
3. Wrong token sent (ensure exact string from generation response)
4. Database connection issues

### Performance Issues

**Solutions:**
1. Add index on `AccessToken.TokenHash`
2. Use connection pooling for database
3. Cache valid tokens for a few seconds
4. Consider read replicas for validation queries

---

**Last Updated:** January 2026
**Version:** 1.0
