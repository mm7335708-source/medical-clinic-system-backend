\# Agent Rules - Medical Clinic Frontend



\## Project Safety Rules



\- Do not refactor the project unless explicitly requested.

\- Do not move files or folders.

\- Do not change UI, styling, routes, or layout during testing tasks.

\- Do not delete real data.

\- Do not modify backend code from this frontend project.

\- Do not change environment files unless explicitly requested.

\- Do not print full JWT access tokens in reports.

\- Do not expose secrets.



\## Testing Data Rules



\- All generated test data must start with: QA Test

\- Test usernames must start with: qat\_

\- Test emails must use: @clinic.local

\- Do not modify or delete the admin user.

\- Do not delete records that were not created by the current test run.

\- If cleanup is unsafe, leave the data and report exactly what remains.



\## API Integration Rules



\- Use src/api/httpClient.ts for all API requests.

\- Do not use raw fetch in service files.

\- Use unwrapApiResponse for ApiResponseDto responses.

\- Use normalizePagedResult for paged endpoints.

\- Preserve existing normalization logic unless a bug is proven.

\- Preserve existing page fallback/mock behavior unless explicitly asked to remove it.



\## Testing Rules



\- Run npm run build before reporting success.

\- During browser testing, verify Network status codes and request payloads.

\- Confirm Authorization: Bearer is attached to protected requests.

\- Report all 400/401/403/404/405/500 responses.

\- For any failed request, report:

&#x20; - module

&#x20; - endpoint

&#x20; - method

&#x20; - status code

&#x20; - request payload

&#x20; - response body

&#x20; - suspected cause

&#x20; - suggested fix



\## Change Rules



\- Prefer report-only mode.

\- If code changes are required, modify the smallest possible file set.

\- Do not modify more than one module per task unless explicitly approved.

\- Always list files changed.

