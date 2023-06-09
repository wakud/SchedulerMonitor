/*----------- Скрипт для програми табла ------------*/
SELECT sa.PointID				--код точки
		, Sp.Name AS PointName	--назва точки
		, sa.Start				--дата початку сеансу
		, sa.Finish				--дата закінчення сеансу
		, PD.PersonName			--ПІБ відвідувача
		, sa.Description		--коментар
FROM dbo.tbServiceAssign sa
LEFT JOIN dbo.tbServicePoint SP ON SP.ID = SA.PointID
LEFT JOIN dbo.vwPersonDiscount PD ON PD.ID  = sa.CardItemID
WHERE 1 = 1
		AND sa.PointID = $pointID$
		--AND CAST(sa.Start AS DATE) = CAST(GETDATE() AS DATE)
		AND CAST(sa.Start AS DATE) = CONVERT(DATE, @dateNow, 103)
ORDER BY sa.Start