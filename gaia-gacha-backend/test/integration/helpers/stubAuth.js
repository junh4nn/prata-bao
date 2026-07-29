// stubAuth.js: fake auth middleware for integration tests. Bypasses real JWT
// verification, attaching a fixed user id so route/business logic can be tested
// without going through requireAuth.js (which already has its own unit tests).

export function createStubAuth(userId) {
  return function stubAuth(req, res, next) {
    req.userId = userId;
    next();
  };
}
