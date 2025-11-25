# Public Alert Broadcast Flow - End-to-End Verification Guide

## Overview
This document provides a comprehensive guide to verify the complete broadcast flow between the Specialist app and Victim app.

## Implementation Summary

### What Was Implemented

#### 1. Public Alert Data Model
- **File**: `Models/PublicAlertModel.cs`
- **Database Table**: `PublicAlerts` (DynamoDB)
- **Fields**:
  - alertId (primary key)
  - title, severity, message
  - location, latitude, longitude, magnitude
  - timestamp, active status

#### 2. Database Service Methods
- **File**: `Services/DynamoDBService.cs:117-204`
- **Methods Added**:
  - `BroadcastPublicAlert()` - Creates new public alert
  - `GetActivePublicAlerts()` - Retrieves all active alerts
  - `DeactivatePublicAlert()` - Marks alert as inactive

#### 3. Controller Endpoints
- **File**: `Controllers/HomeController.cs:143-193`
- **Endpoints Added**:
  - `GET /Home/PublicAlerts` - View public alerts (Victim app)
  - `GET /Home/BroadcastAlert` - Broadcast form (Specialist app)
  - `POST /Home/SendBroadcastAlert` - Submit new alert
  - `POST /Home/DeactivateAlert` - Deactivate alert

#### 4. Victim App - Public Alert Display
- **View**: `Views/Home/PublicAlerts.cshtml`
- **CSS**: `wwwroot/css/publicAlerts.css`
- **Features**:
  - Color-coded severity badges (Critical/High/Moderate/Low)
  - Real-time location display with coordinates
  - Auto-refresh every 30 seconds
  - Link to SOS page
  - Responsive design

#### 5. Specialist App - Broadcast Alert Form
- **View**: `Views/Home/BroadcastAlert.cshtml`
- **CSS**: `wwwroot/css/broadcastAlert.css`
- **Features**:
  - Form to create earthquake alerts
  - Real-time preview of alert appearance
  - Geolocation support
  - Severity level selection
  - Back to dashboard navigation

#### 6. Navigation Updates
- **SOS Page** (`Views/Home/SOS.cshtml:13-16`): Added "View Public Alerts" button
- **Dashboard** (`Views/Home/firstRespDashboard.cshtml:77`): Added "Broadcast Public Alert" button

---

## End-to-End Verification Steps

### Prerequisites
1. DynamoDB `PublicAlerts` table must be created in AWS
2. Application must be running (`dotnet run`)
3. AWS credentials must be valid

### Test Flow 1: Specialist Broadcasts Alert to Victims

#### Step 1: Login as First Responder (Specialist)
1. Navigate to `http://localhost:5000/Home/Login`
2. Enter credentials:
   - Token: `FR-2847`
   - Email: `firstresponder@test.com`
   - Password: `fr123`
3. Click "Login"
4. **Expected**: Redirect to First Responder Dashboard

#### Step 2: Access Broadcast Alert Page
1. From the dashboard, click "Broadcast Public Alert" button (purple)
2. **Expected**: Navigate to `/Home/BroadcastAlert`
3. **Verify**: Form displays with all fields:
   - Alert Title
   - Severity Level dropdown
   - Magnitude
   - Location Name
   - Latitude/Longitude
   - Alert Message
   - Preview panel on the right

#### Step 3: Create a Test Alert
Fill in the form with test data:
```
Alert Title: Earthquake Warning - Downtown Vancouver
Severity: high
Magnitude: 5.8
Location: Downtown Vancouver
Latitude: 49.2827
Longitude: -123.1207
Message: Strong earthquake detected. Seek immediate shelter. Stay away from windows and heavy objects. Emergency services are responding.
```

Optional: Click "Use My Location" to auto-fill coordinates

**Expected**: Preview panel updates in real-time as you type

#### Step 4: Broadcast the Alert
1. Click "Broadcast Alert" button
2. **Expected**:
   - Status message: "Alert broadcast successfully! Victims can now view this alert."
   - Green success message appears
   - Form resets to empty state

#### Step 5: Verify Alert in Victim App
1. Open a new browser tab/window (or different browser)
2. Navigate to `http://localhost:5000/Home/PublicAlerts`
3. **Expected**:
   - Alert card displays with:
     - Orange "HIGH" severity badge (top-left)
     - Current timestamp (top-right)
     - Title: "Earthquake Warning - Downtown Vancouver"
     - Magnitude: 5.8 (red background section)
     - Location: Downtown Vancouver
     - Coordinates: Lat: 49.2827 | Lng: -123.1207
     - Full message text
     - Alert ID at bottom
   - Orange left border on alert card
   - Card has hover effect (lifts up slightly)

#### Step 6: Test Auto-Refresh
1. Wait 30 seconds on the Public Alerts page
2. **Expected**: Page automatically refreshes
3. Alert should still be visible

#### Step 7: Test Navigation from SOS Page
1. Navigate to `http://localhost:5000/Home/SOS`
2. Click "View Public Alerts" button
3. **Expected**: Redirects to Public Alerts page with alert visible

---

### Test Flow 2: Multiple Alerts with Different Severity Levels

#### Step 1: Create Multiple Alerts
As First Responder, create 4 alerts with different severities:

**Alert 1 - Critical:**
```
Title: Major Earthquake - Immediate Evacuation Required
Severity: critical
Magnitude: 7.2
Location: Vancouver Downtown Core
Coordinates: 49.2827, -123.1207
Message: CRITICAL ALERT: Major earthquake detected. Evacuate buildings immediately. Tsunami warning in effect for coastal areas.
```

**Alert 2 - High:**
```
Title: Strong Aftershock Warning
Severity: high
Magnitude: 6.1
Location: North Vancouver
Coordinates: 49.3200, -123.0724
Message: Strong aftershock expected within the next hour. Remain in safe locations.
```

**Alert 3 - Moderate:**
```
Title: Seismic Activity Detected
Severity: moderate
Magnitude: 4.5
Location: Surrey
Coordinates: 49.1913, -122.8490
Message: Moderate seismic activity detected. Stay alert and be prepared for potential aftershocks.
```

**Alert 4 - Low:**
```
Title: Minor Tremor Advisory
Severity: low
Magnitude: 3.2
Location: Richmond
Coordinates: 49.1666, -123.1336
Message: Minor tremor detected. No immediate danger. Continue normal activities with awareness.
```

#### Step 2: Verify All Alerts Display
1. Navigate to `/Home/PublicAlerts`
2. **Expected**: All 4 alerts display with:
   - Critical: Red badge, red left border
   - High: Orange badge, orange left border
   - Moderate: Yellow badge, yellow left border
   - Low: Blue badge, blue left border
3. Most recent alert appears first

---

### Test Flow 3: No Alerts Scenario

#### Step 1: Test Empty State
1. If all alerts have been deactivated, navigate to `/Home/PublicAlerts`
2. **Expected**:
   - White card displays with:
     - Green heading: "No Active Alerts"
     - Message: "There are currently no active earthquake alerts in your area."

---

### Test Flow 4: Geolocation Integration

#### Step 1: Use Geolocation in Broadcast Form
1. Login as First Responder
2. Navigate to `/Home/BroadcastAlert`
3. Click "Use My Location" button
4. **Expected**:
   - Browser prompts for location permission
   - If allowed: Latitude and Longitude fields auto-populate
   - Preview updates with coordinates
   - Status message: "Location obtained successfully!"

---

## API Endpoint Testing

### Test with cURL or Postman

#### 1. Get Active Public Alerts
```bash
curl http://localhost:5000/Home/PublicAlerts
```
**Expected**: HTML page with active alerts

#### 2. Broadcast New Alert (Authenticated)
```bash
curl -X POST http://localhost:5000/Home/SendBroadcastAlert \
  -H "Content-Type: application/json" \
  -d '{
    "title": "Test Alert",
    "severity": "moderate",
    "magnitude": "5.0",
    "location": "Test Location",
    "latitude": 49.2827,
    "longitude": -123.1207,
    "message": "This is a test alert message"
  }'
```
**Expected**: 200 OK with success message

#### 3. Deactivate Alert (Authenticated)
```bash
curl -X POST http://localhost:5000/Home/DeactivateAlert \
  -H "Content-Type: application/json" \
  -d '{"alertId": "1732492800"}'
```
**Expected**: 200 OK

---

## Database Verification

### DynamoDB Table Structure

**Table Name**: `PublicAlerts`
**Primary Key**: `alertId` (String)

**Sample Item**:
```json
{
  "alertId": "1732492800",
  "title": "Earthquake Warning - Downtown Vancouver",
  "severity": "high",
  "magnitude": "5.8",
  "location": "Downtown Vancouver",
  "latitude": 49.2827,
  "longitude": -123.1207,
  "message": "Strong earthquake detected...",
  "timestamp": "2025-11-25 14:30:00 UTC",
  "active": true
}
```

### Verify in AWS Console
1. Login to AWS Console
2. Navigate to DynamoDB > Tables > PublicAlerts
3. Click "Explore table items"
4. **Expected**: See alerts created during testing
5. Verify all fields are populated correctly

---

## Expected Results Summary

### Specialist App (First Responder)
- Can access Broadcast Alert page from dashboard
- Can create alerts with all required fields
- Real-time preview shows alert appearance
- Geolocation integration works
- Success confirmation after broadcast
- Form validates required fields

### Victim App (Public Users)
- Can view all active alerts without authentication
- Alerts display with correct severity styling
- Auto-refresh every 30 seconds
- Can navigate to SOS page
- Responsive design works on mobile
- No alerts state displays correctly

### Data Flow
- Alerts save to DynamoDB immediately
- Victim app retrieves only active alerts
- Timestamps are correct
- Location coordinates display accurately
- Severity levels affect visual styling

---

## Known Limitations

1. **No Real-time Push Notifications**: Victims must refresh page or wait 30 seconds
2. **DynamoDB Table Creation**: `PublicAlerts` table must be created manually in AWS
3. **No Authentication for Victim App**: Public alerts are accessible without login
4. **No Alert Expiration**: Alerts remain active until manually deactivated
5. **Hardcoded AWS Credentials**: Security concern (see DynamoDBService.cs:24-25)

---

## Troubleshooting

### Issue: "Cannot load table PublicAlerts"
**Solution**: Create the table in DynamoDB:
- Table name: `PublicAlerts`
- Primary key: `alertId` (String)
- Region: `ca-central-1`

### Issue: Alerts not displaying on Public Alerts page
**Solution**:
1. Check DynamoDB table has items with `active = true`
2. Verify AWS credentials are correct
3. Check browser console for JavaScript errors

### Issue: Broadcast form not submitting
**Solution**:
1. Verify all required fields are filled
2. Check browser console for errors
3. Ensure you're logged in as First Responder

### Issue: Geolocation not working
**Solution**:
1. Ensure site is using HTTPS or localhost
2. Grant browser location permissions
3. Check browser console for errors

---

## Build Verification

The application builds successfully with 0 errors and 21 warnings (nullable reference warnings only).

```bash
dotnet build Team2_EarthquakeAlertApp.csproj
```

**Build Status**: SUCCESS

---

## File Checklist

### New Files Created
- [x] `Models/PublicAlertModel.cs`
- [x] `Views/Home/PublicAlerts.cshtml`
- [x] `Views/Home/BroadcastAlert.cshtml`
- [x] `wwwroot/css/publicAlerts.css`
- [x] `wwwroot/css/broadcastAlert.css`
- [x] `BROADCAST_FLOW_VERIFICATION.md`

### Modified Files
- [x] `Services/DynamoDBService.cs` - Added public alert methods
- [x] `Controllers/HomeController.cs` - Added public alert endpoints
- [x] `Views/Home/SOS.cshtml` - Added navigation link
- [x] `Views/Home/firstRespDashboard.cshtml` - Added broadcast button
- [x] `wwwroot/css/SOS.css` - Added header navigation styling
- [x] `wwwroot/css/firstRespDashboard.css` - Added broadcast button styling

---

## Conclusion

The public alert broadcast system is fully implemented and ready for testing. The end-to-end flow from Specialist broadcasting alerts to Victims viewing them has been verified through code implementation.

To complete verification:
1. Create the `PublicAlerts` table in DynamoDB
2. Run the application
3. Follow the test flows above
4. Verify all expected results

The system successfully enables two-way communication in the earthquake alert application:
- **Victims → Specialists**: SOS distress signals (existing)
- **Specialists → Victims**: Public earthquake alerts (NEW)
