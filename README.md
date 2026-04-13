# 산업 통합 운영 대시보드 (Industrial-CM)

산업 설비 운영 시나리오을 위한 WPF기반 데모 웹 애플리케이션입니다.
로그인, 대시보드, 설비/센서 모니터링, 원격제어, 알람 로그, 사용자/권한 설정 화면을 제공합니다

## 개요
Industrial-CM은 제조, 산업 현장 운영 화면을 구성하고 데스크톱 기반의 운영 앱의 기본구조(MVVM, Verify, 저장소 분리)를 검증하기 위한 프로젝트입니다.

## 주요 기능
- 사용자 로그인/로그아웃
  - 사용자 인증 후 주요 기능 접근 허용
  - 로그인 하지 않은 상태 -> 이동 제한 
- 운영 대시보드
  - KPI 카드, 생산 오더, 재고 상태, 알림, 액션 항목 표시
  - 운영 현환을 대시보드 화면에서 확인 가능
- 설비/장비 목록 조회
  - 장비명, 상태, Heart Beat 시간 표시
- 센서 데이터 모니터링
  - 장비별 센서 타입, 값, 단위, 수집 시각 표시
- 장비 원격 제어 (DEMO scenario)
  - Start, Stop, Reset 명령 시뮬레이션
  - 진행률 표시 밒 제어 로그 조회
  - Start, Stop 시 일정 확률로 Fault발생 (10%), RESET으로 복구 시나리오 제공
- 알람/이벤트 로그 조회
  - 발생 시각, 심각도, 원인지, 메세지 확인
- 사용자/권한 조회
  - 사용자 목록 및 역할 표시
- 설정 화면
  - PLC, Gateway 연결 값 입력 및 검증 포맷 제공
  - 비밀번호 변경 기능
    - 특정 조건 (번호 8자리 이상, 문자 1자 이상 포함, 연속/반복 숫자 3자리 금지) 추가

## 기술 스택
- Frontend: WPF(XAML), MVVM
- Backend: C#, .NET 8, Entity Framework
- DB: SQL, InMemory

## 데모 계정
- viewer / viewer123 (Viewer)
- operator / operator123 (Operator)

## 제한 사항
- 일부 데이터는 데모 시연용 가상 데이터 입니다.
- 실제 PLC/센서 실시간 통신은 지원하지 않습니다.
- 인증 및 비밀번호는 운영 보안(해시,암호화)으로 구현되지 않았습니다.


## 빠른 시작
``` bash
dotnet restore
dotnet build IndustrialIotManager.sln
dotnet run --project IndustrialIotManager/IndustrialIotManager.csproj
```

## 실행 환경
- Window 10 이상
- .NET sdk 8.0 이상
- MAC OS / Linux 환경에서는 실행이 어렵습니다.

